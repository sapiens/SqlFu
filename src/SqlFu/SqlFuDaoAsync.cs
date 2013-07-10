using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model;
using SqlFu.Expressions;

namespace SqlFu
{
    public static class SqlFuDaoAsync
    {

        public static async Task<List<T>> FetchAsync<T>(this DbConnection cnx,CancellationToken token, string sql, params object[] args) 
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return await cmd.FetchAsync<T>(token);
            }
        }

        public static async Task<List<T>> FetchAsync<T>(this DbCommand cmd, CancellationToken cancellation,Func<IDataReader, T> mapper = null,
                                       bool firstRowOnly = false)
        {
            List<T> rez = new List<T>();
            
            try
            {
                CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
                using (var reader = await cmd.ExecuteReaderAsync(behavior,cancellation))
                {
                    SqlFuDao.OnCommand(cmd);
                    while (await reader.ReadAsync(cancellation))
                    {
                        if (mapper == null)
                        {
                            mapper = PocoFactory.GetPocoMapper<T>(reader, cmd.CommandText);
                        }
                        rez.Add(mapper(reader));
                    }
                }

                return rez;
            }
            catch (DbException ex)
            {
                SqlFuDao.OnException(cmd, ex);
                throw;
            }
        }

        /// <summary>
        /// Returns only the first row of result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task<T> QuerySingleAsync<T>(this DbConnection cnx, CancellationToken token,string sql, params object[] args) 
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                var data=await cmd.FetchAsync<T>(token,firstRowOnly: true);
                return data.FirstOrDefault();
            }
        }

        /// <summary>
        /// Selects first row matching criteria and maps it to poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(this DbConnection db,Expression<Func<T, bool>> condition,CancellationToken token) 
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectAllColumns()
                .WriteFrom()
                .Where(condition);
            return await db.QuerySingleAsync<T>(token,builder.ToString(), builder.Parameters.ToArray());
        }

         /// <summary>
         /// Selects first row matching criteria and maps it to poco
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <param name="db"></param>
         /// <param name="condition"></param>
         /// <returns></returns>
         public static async Task<T> GetAsync<T>(this DbConnection db, Expression<Func<T, bool>> condition)
         {
             return await db.GetAsync(condition, CancellationToken.None);
         }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> QueryAsync<T>(this DbConnection db, Expression<Func<T, bool>> condition,CancellationToken token) 
        {
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectAllColumns()
                .WriteFrom()
                .Where(condition);
            return await db.FetchAsync<T>(token,builder.ToString(), builder.Parameters.ToArray());
        }

        public static async Task<IEnumerable<T>> QueryAsync<T>(this DbConnection db, Expression<Func<T, bool>> condition) 
        {
            return await db.QueryAsync(condition, CancellationToken.None);
        }


        public static async Task<int> ExecuteAsync(this DbConnection cnx, CancellationToken token,string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return await cmd.ExecuteAsync(token);
            }
        }


        public static async Task<int> ExecuteAsync(this DbConnection cnx, string sql,params object[] args)
        {
            return await cnx.ExecuteAsync(CancellationToken.None, sql, args);
        }

        public static async Task<int> ExecuteAsync(this DbCommand cmd,CancellationToken token)
        {
            int rez;
            try
            {
                rez = await cmd.ExecuteNonQueryAsync(token);
                SqlFuDao.OnCommand(cmd);
                return rez;
            }
            catch (Exception ex)
            {
                SqlFuDao.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task<T> GetValueAsync<T>(this DbConnection cnx, CancellationToken token,string sql, params object[] args)
        {
            using (var cmd = cnx.CreateAndSetupCommand(sql, args))
            {
                return await cmd.GetValueAsync<T>(PocoFactory.GetConverter<T>(),token);
            }
        }

        public static async Task<T> GetValueAsync<T>(this DbConnection cnx, string sql,
                                                        params object[] args)
        {
            return await cnx.GetValueAsync<T>(CancellationToken.None, sql, args);
        }

        public static async Task<T> GetValueAsync<T>(this DbCommand cmd,Func<object, T> converter,CancellationToken token)
        {
            object rez;
            try
            {
                rez = await cmd.ExecuteScalarAsync(token);
                SqlFuDao.OnCommand(cmd);
                return converter(rez);
            }
            catch (Exception ex)
            {
                SqlFuDao.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task<PagedResult<T>> PagedQueryAsync<T>(this DbConnection cnx, long skip, int take, string sql,
                                                                    params object[] args)
        {
            return await cnx.PagedQueryAsync<T>(CancellationToken.None, skip, take, sql, args);
        }


        public static async Task<PagedResult<T>> PagedQueryAsync<T>(this DbConnection cnx,CancellationToken token, long skip, int take, string sql,
                                                  params object[] args)
        {
            var rez = new PagedResult<T>();
            using (var cmd = cnx.CreateCommand())
            {
                var statement = new PreparePagedStatement(cnx.GetProvider(), skip, take, sql, args);
                statement.SetupCount(cmd);
                try
                {
                    var cnt = await cmd.ExecuteScalarAsync(token);
                    cnt.MustNotBeNull();
                    if (cnt.GetType() == typeof(Int32))
                    {
                        rez.Count = (int)cnt;
                    }
                    else
                    {
                        rez.LongCount = (long)cnt;
                    }
                    SqlFuDao.OnCommand(cmd);
                }
                catch (Exception ex)
                {
                    SqlFuDao.OnException(cmd, ex);
                    throw;
                }

                if (rez.Count > 0)
                {
                    statement.Setup(cmd);
                    rez.Items = await cmd.FetchAsync<T>(token);
                }

                return rez;
            }
        }


        /// <summary>
        /// Returns only the specified column value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <param name="db"></param>
        /// <param name="selector">Column selector</param>
        /// <param name="criteria">Selection criteria</param>
        /// <returns></returns>
        public static async Task<R> GetColumnValueAsync<T, R>(this DbConnection db, Expression<Func<T, R>> selector,
                                             Expression<Func<T, bool>> criteria,CancellationToken token)
        {
            selector.MustNotBeNull();
            var builder = new ExpressionSqlBuilder<T>(db.GetProvider().BuilderHelper);
            builder
                .WriteSelect()
                .WriteSelectColumn(selector)
                .WriteFrom()
                .Where(criteria);

            return await db.GetValueAsync<R>(token,builder.ToString(), builder.Parameters.ToArray());
        }

         /// <summary>
         /// Returns only the specified column value
         /// </summary>
         /// <typeparam name="T"></typeparam>
         /// <typeparam name="R"></typeparam>
         /// <param name="db"></param>
         /// <param name="selector">Column selector</param>
         /// <param name="criteria">Selection criteria</param>
         /// <returns></returns>
         public static async Task<R> GetColumnValueAsync<T, R>(this DbConnection db, Expression<Func<T, R>> selector,
                                                               Expression<Func<T, bool>> criteria)
         {
             return await db.GetColumnValueAsync(selector, criteria, CancellationToken.None);
         }
    }
}