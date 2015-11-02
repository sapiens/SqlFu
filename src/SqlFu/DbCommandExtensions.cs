using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Executors;

namespace SqlFu
{
    public static class DbCommandExtensions
    {
        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, string sql, params object[] args)
        {
            return CreateAndSetupCommand(cnx, new CommandConfiguration(sql, args));
        }

        /// <summary>
        /// Sets the commands sql and parameters, takes care of provider specific functionality. At the end, the command is ready to execute
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, CommandConfiguration cfg)
        {
            var provider = cnx.GetProvider();
            var cmd = cnx.CreateCommand();
            cmd.CommandText = cfg.SqlText;
            if (cfg.IsStoredProcedure) cmd.CommandType=CommandType.StoredProcedure;
            cmd.CommandText = provider.FormatSql(cmd.CommandText, cmd.SetupParameters(provider,cfg.Args));
            provider.OnCommandExecuting(cmd);
            cfg.ApplyOptions(cmd);
            return cmd;
        }

        public static int Execute(this DbCommand cmd)
        {
            int rez;
            try
            {
                rez = cmd.ExecuteNonQuery();
                SqlFuManager.Config.OnCommand(cmd);
                return rez;
            }
            catch (Exception ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }
       

        /// <summary>
        /// Gets one result value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static T GetValue<T>(this DbCommand cmd, Func<object, T> converter = null)
        {
            try
            {
                object rez = cmd.ExecuteScalar();
                SqlFuManager.Config.OnCommand(cmd);
                return SqlFuManager.GetConverter(converter)(rez);
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        /// <summary>
        /// Great of huge result sets, process one row at the time
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="processor">Result processor</param>
        /// <param name="mapper"></param>
        /// <param name="firstRowOnly"></param>
        public static void QueryAndProcess<T>(this DbCommand cmd, Func<T, bool> processor, Func<IDataReader, T> mapper = null,
                                     bool firstRowOnly = false)
        {
            try
            {
                CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
                using (var reader = cmd.ExecuteReader(behavior))
                {
                    SqlFuManager.Config.OnCommand(cmd);

                    while (reader.Read())
                    {
                        if (!processor(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader))) break;
                    }
                }


            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        //public static IEnumerable<T> Query<T>(this DbCommand cmd, Func<IDataReader, T> mapper = null,
        //                                   bool firstRowOnly = false)
        //{
        //    DbDataReader reader = null;

        //    try
        //    {
        //        CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;

        //        reader = cmd.ExecuteReader(behavior);
        //    }
        //    catch (DbException ex)
        //    {
               
        //        SqlFuManager.Config.OnException(cmd, ex);
        //        throw;
        //    }

        //    SqlFuManager.Config.OnCommand(cmd);
        //    var hasRows = true;
        //    T poco = default(T);
        //    while (hasRows)
        //    {
        //        try
        //        {
        //            hasRows = reader.Read();
        //            poco = SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader);
        //        }
        //        catch (DbException ex)
        //        {
        //            reader.Close();
        //            SqlFuManager.Config.OnException(cmd, ex);
        //            throw;
        //        }
        //        catch (Exception)
        //        {
        //            reader.Close();
        //            throw;
        //        }
        //        yield return poco;
        //        if (firstRowOnly) yield break;
        //    }
           
        //    reader.Close();
        //}

        public static List<T> Fetch<T>(this DbCommand cmd, Func<IDataReader, T> mapper = null,
                                    bool firstRowOnly = false)
        {
            var rez = firstRowOnly ? new List<T>(1) : new List<T>();

            cmd.QueryAndProcess(p =>
            {
                rez.Add(p);
                return true;
            },mapper,firstRowOnly);
            return rez;
            //try
            //{
            //    CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
            //    using (var reader = cmd.ExecuteReader(behavior))
            //    {
            //        SqlFuManager.Config.OnCommand(cmd);
            //        while (reader.Read())
            //        {
            //            rez.Add(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader));
            //            if (firstRowOnly) break;
            //        }
            //    }

            //    return rez;
            //}
            //catch (DbException ex)
            //{
            //    SqlFuManager.Config.OnException(cmd, ex);
            //    throw;
            //}
        }

        #region Async

      

        public static async Task<int> ExecuteAsync(this DbCommand cmd, CancellationToken token)
        {
            int rez;
            try
            {
                rez = await cmd.ExecuteNonQueryAsync(token).ConfigureAwait(false);
                SqlFuManager.Config.OnCommand(cmd);
                return rez;
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }


        public static async Task<T> GetValueAsync<T>(this DbCommand cmd, CancellationToken token, Func<object, T> converter = null)
        {
            try
            {
                object rez = await cmd.ExecuteScalarAsync(token).ConfigureAwait(false);
                SqlFuManager.Config.OnCommand(cmd);
                return SqlFuManager.GetConverter(converter)(rez);
            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }
        /// <summary>
        /// Executes an async query then processes each result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cmd"></param>
        /// <param name="cancellation"></param>
        /// <param name="processor">Delegate to process a result item. Return false from delegate to end the query</param>
        /// <param name="mapper"></param>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        public static async Task QueryAndProcessAsync<T>(this DbCommand cmd, CancellationToken cancellation, Func<T, bool> processor, Func<IDataReader, T> mapper = null,
                                      bool firstRowOnly = false)
        {
            try
            {
                CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
                using (var reader = await cmd.ExecuteReaderAsync(behavior, cancellation).ConfigureAwait(false))
                {
                    SqlFuManager.Config.OnCommand(cmd);

                    while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
                    {
                        if (!processor(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader))) break;
                    }
                }


            }
            catch (DbException ex)
            {
                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task<List<T>> FetchAsync<T>(this DbCommand cmd, CancellationToken cancellation, Func<IDataReader, T> mapper = null,
                                      bool firstRowOnly = false)
        {
            var rez = firstRowOnly ? new List<T>(1) : new List<T>();
            await cmd.QueryAndProcessAsync(cancellation, d =>
            {
                rez.Add(d);
                return true;
            },mapper,firstRowOnly);
            return rez;
            //try
            //{
            //    CommandBehavior behavior = firstRowOnly ? CommandBehavior.SingleRow : CommandBehavior.Default;
            //    using (var reader = await cmd.ExecuteReaderAsync(behavior, cancellation).ConfigureAwait(false))
            //    {
            //        SqlFuManager.Config.OnCommand(cmd);
            //        while (await reader.ReadAsync(cancellation).ConfigureAwait(false))
            //        {
            //            rez.Add(SqlFuManager.GetMapper(mapper, cmd.CommandText)(reader));
            //            if (firstRowOnly) break;
            //        }
            //    }

            //    return rez;
            //}
            //catch (DbException ex)
            //{
            //    SqlFuManager.Config.OnException(cmd, ex);
            //    throw;
            //}
        } 
        #endregion
    }
}