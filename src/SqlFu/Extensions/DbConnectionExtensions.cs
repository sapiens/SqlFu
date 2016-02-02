using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu
{
    public static class DbConnectionExtensions
    {
        public static int Execute(this DbConnection cnx, CommandConfiguration cfg)
        {
            using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                return cmd.Execute();
            }
        }

        public static async Task<int> ExecuteAsync(this DbConnection cnx, CommandConfiguration cfg, CancellationToken token)
        {
            using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                return await cmd.ExecuteAsync(token).ConfigureAwait(false);
            }
        }

        public static int Execute(this DbConnection cnx, Action<IConfigureCommand> cfg)
        {
            var cmd = new CommandConfiguration();
            cfg(cmd);
            return cnx.Execute(cmd);
        }

        public static int Execute(this DbConnection cnx, string sql, params object[] args)
            => cnx.Execute(c => c.Sql(sql, args));

        public static Task<int> ExecuteAsync(this DbConnection cnx, CancellationToken token,string sql, params object[] args)
            => cnx.ExecuteAsync(c => c.Sql(sql, args), token);
        

        public static Task<int> ExecuteAsync(this DbConnection cnx, Action<IConfigureCommand> cfg, CancellationToken token)
        {
            var cmd = new CommandConfiguration();
            cfg(cmd);
            return cnx.ExecuteAsync(cmd,token);
        }

        public static T GetValue<T>(this DbConnection db, CommandConfiguration cfg)
        {
            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                return cmd.GetValue<T>();
            }
            
        }

        public static T GetValue<T>(this DbConnection db, Action<IConfigureCommand> cfg)
        {
            var cmd=new CommandConfiguration();
            cfg(cmd);        
            return db.GetValue<T>(cmd);
        }

        public static T GetValue<T>(this DbConnection cnx, string sql, params object[] args)
          => cnx.GetValue<T>(c => c.Sql(sql, args));

        public static Task<T> GetValueAsync<T>(this DbConnection cnx,CancellationToken token ,string sql, params object[] args)
          => cnx.GetValueAsync<T>(c => c.Sql(sql, args),token);

        public static async Task<T> GetValueAsync<T>(this DbConnection db, CommandConfiguration cfg, CancellationToken token)
        {
            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                return await cmd.GetValueAsync<T>(token).ConfigureAwait(false);
            }
        }

        public static Task<T> GetValueAsync<T>(this DbConnection db, Action<IConfigureCommand> cfg, CancellationToken token)
        {
            var cmd=new CommandConfiguration();
            cfg(cmd);
            return GetValueAsync<T>(db,cmd, token);
        }


        internal static async Task QueryAndProcessAsync<T>(this DbConnection db, CommandConfiguration cfg, Func<T, bool> processor, CancellationToken token,T anonModel = default(T),
            bool firstRowOnly = false) 
        {
        
            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                await cmd.QueryAndProcessAsync(token,processor,firstRowOnly:firstRowOnly).ConfigureAwait(false);
            }
        }

        ///// <summary>
        ///// Gets a single row then maps it to a poco
        ///// </summary>
        //public static T QuerySingle<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder) where T : class
        //{
        //    var cc = builder(db.GetSqlBuilder());
        //    return db.GetSingle<T>(c=>c.Import(cc.GetCommandConfiguration()));
        //}

        ///// <summary>
        ///// Gets a single row then maps it to a poco
        ///// </summary>
        //public static Task<T> QuerySingleAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder,CancellationToken token) where T : class
        //{
        //    var cc = builder(db.GetSqlBuilder());
        //    return db.GetSingleAsync<T>(c=>c.Import(cc.GetCommandConfiguration()),token);
        //}

        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cfg"></param>
        /// <param name="anonModel">Anonymous type</param>
        /// <returns></returns>
        public static T FetchSingle<T>(this DbConnection db, Action<IConfigureCommand> cfg, T anonModel = null) where T : class
        {
            T rez = null;
             db.QueryAndProcess(cfg, d =>
            {
                rez = d;
                return false;
            },anonModel,true);
            return rez;
        }

        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="anonModel">Anonymous type</param>
        /// <param name="cfg"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> FetchSingleAsync<T>(this DbConnection db, Action<IConfigureCommand> cfg, CancellationToken token, T anonModel = null) where T : class
        {
            T rez = null;
            await db.QueryAndProcessAsync(cfg, d =>
           {
               rez = d;
               return false;
           }, token, anonModel,true);
            return rez;
        }



        public static List<T> Fetch<T>(this DbConnection cnx, Action<IConfigureCommand> cfg, T anonModel = default(T))
        {
            var list=new List<T>();
            cnx.QueryAndProcess(cfg,d=> { list.Add(d);
                                            return true;
            },anonModel);
            return list;
        }
        public static async Task<List<T>> FetchAsync<T>(this DbConnection cnx, Action<IConfigureCommand> cfg,CancellationToken token ,T anonModel = default(T))
        {
            var list=new List<T>();
            await cnx.QueryAndProcessAsync(cfg,d=> { list.Add(d);
                                            return true;
            },token,anonModel);
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg">Sql and command arguments</param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="anonModel">Map to</param>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        public static void QueryAndProcess<T>(this DbConnection cnx, Action<IConfigureCommand> cfg,
            Func<T, bool> processor, T anonModel = default(T),
            bool firstRowOnly = false) 
        {
            var cmdConfig = new CommandConfiguration();
            cfg(cmdConfig);
            cnx.QueryAndProcess(cmdConfig,processor,anonModel,firstRowOnly);
        }

        public static void QueryAndProcess<T>(this DbConnection cnx, Func<IBuildQueryFrom, IGenerateSql<T>> sqlBuilder,
            Func<T, bool> processor, bool firstRowOnly = false)
        {
            cnx.QueryAndProcess(sqlBuilder(cnx.GetSqlBuilder()).GetCommandConfiguration(), processor, default(T), firstRowOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg">Sql and command arguments</param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="token"></param>
        /// <param name="anonModel">Map to</param>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        public static Task QueryAndProcessAsync<T>(this DbConnection cnx, Action<IConfigureCommand> cfg,
            Func<T, bool> processor,CancellationToken token, T anonModel = default(T),
            bool firstRowOnly = false)
        {
            var cmdConfig = new CommandConfiguration();
            cfg(cmdConfig);
            return cnx.QueryAndProcessAsync(cmdConfig,processor,token,anonModel,firstRowOnly);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="sqlBuilder">Sql builder</param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="token"></param>
        /// <param name="firstRowOnly"></param>
        /// <returns></returns>
        public static Task QueryAndProcessAsync<T>(this DbConnection cnx, Func<IBuildQueryFrom, IGenerateSql<T>> sqlBuilder,
           Func<T, bool> processor, CancellationToken token,
           bool firstRowOnly = false)
        {
            return cnx.QueryAndProcessAsync(sqlBuilder(cnx.GetSqlBuilder()).GetCommandConfiguration(), processor, token,default(T), firstRowOnly);
        }


        internal static void QueryAndProcess<T>(this DbConnection cnx, CommandConfiguration cfg,Func<T, bool> processor, T anonModel =default(T),
           bool firstRowOnly=false)
        {
           using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                cmd.QueryAndProcess(processor,firstRowOnly:firstRowOnly);
            }
        }


        /// <summary>
        /// Returns the escaped table name for that poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this DbConnection cnx)
        {
            return SqlFuManager.Config.TableInfoFactory.GetInfo(typeof(T)).EscapeName(cnx.GetProvider());
        }

        public static PagedResult<T> FetchPaged<T>(this DbConnection db,Action<IConfigureCommand> cfg,Pagination page,T anonModel=null) where T:class
        {
            typeof(T).Must(t=>t!=typeof(object),"Dynamic types are not supported");
            var cmd=new CommandConfiguration();
            cfg(cmd);
            var builder = new PagedQueryBuilder(db.GetPocoInfo<T>(), db.GetProvider());
            var data = builder.Build(cmd.SqlText, cmd.Args, page);

            var result=new PagedResult<T>();
            result.LongCount =
                db.GetValue<long>(c => c.Sql(data.CountSql, cmd.Args).WithCommandOptions(cmd.ApplyOptions));
            if (result.Count == 0) return result;

            result.Items = db.Fetch<T>(c => c.Sql(data.PagedSql, data.Args).WithCommandOptions(cmd.ApplyOptions));
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder">Don't use Limit, pagination must be specified in the next argument</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static PagedResult<T> QueryPaged<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder,Pagination page) where T : class
        {
            var cfg = builder(db.GetSqlBuilder()).GetCommandConfiguration();
            return db.FetchPaged<T>(c => c.Sql(cfg.SqlText, cfg.Args), page);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder">Don't use Limit, pagination must be specified in the next argument</param>
        /// <param name="page"></param>
        /// <param name="token"></param>
        public static Task<PagedResult<T>> QueryPagedAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder,Pagination page,CancellationToken token) where T : class
        {
            var cfg = builder(db.GetSqlBuilder()).GetCommandConfiguration();
            return db.FetchPagedAsync<T>(c => c.Sql(cfg.SqlText, cfg.Args), page,token);
        }

         public static async Task<PagedResult<T>> FetchPagedAsync<T>(this DbConnection db,Action<IConfigureCommand> cfg,Pagination page,CancellationToken token,T anonModel=null) where T:class
        {
            typeof(T).Must(t => t != typeof(object), "Dynamic types are not supported");
            var cmd=new CommandConfiguration();
            cfg(cmd);
            var builder = new PagedQueryBuilder(db.GetPocoInfo<T>(), db.GetProvider());
            var data = builder.Build(cmd.SqlText, cmd.Args, page);

            var result=new PagedResult<T>();
            result.LongCount = await 
                db.GetValueAsync<long>(c => c.Sql(data.CountSql, cmd.Args).WithCommandOptions(cmd.ApplyOptions),token).ConfigureAwait(false);
            if (result.Count == 0) return result;

            result.Items = await db.FetchAsync<T>(c => c.Sql(data.PagedSql, data.Args).WithCommandOptions(cmd.ApplyOptions),token).ConfigureAwait(false);
            return result;
        }

        #region Strongly typed queries

        /// <summary>
        /// Used to build a query string using expressions
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IBuildQueryFrom GetSqlBuilder(this DbConnection db)=> new SqlFrom(db.GetProvider(),db.SqlFuConfig().TableInfoFactory);
        

        public static T GetQueryValue<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder)
        {
            return db.GetValue<T>(builder(db.GetSqlBuilder()).GetCommandConfiguration());
        }

        public static Task<T> GetQueryValueAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder,CancellationToken token)
        {
            return db.GetValueAsync<T>(builder(db.GetSqlBuilder()).GetCommandConfiguration(),token);
        }

        public static T GetSingle<T>(this DbConnection db, Expression<Func<T, bool>> criteria=null,Expression<Func<T, object>> orderBy=null)
        {
            return db.GetSingle(d =>

                d.From<T>().Where(criteria ?? (f => 1 == 1))
                    .OrderBy(orderBy ?? (f => 1))
                    .Limit(1)
                    .AllColumns()
                );
        }

        public static Task<T> GetSingleAsync<T>(this DbConnection db, CancellationToken cancel,Expression<Func<T, bool>> criteria=null,Expression<Func<T, object>> orderBy=null)
        {
            return db.GetSingleAsync(d =>

                d.From<T>().Where(criteria ?? (f => 1 == 1))
                    .OrderBy(orderBy ?? (f => 1))
                    .Limit(1)
                    .AllColumns()
                ,cancel);

        }

        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>    
        /// <returns></returns>
        public static T GetSingle<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder)
        {
            builder.MustNotBeNull();
            var built = builder(db.GetSqlBuilder());
            T rez = default(T);
            db.QueryAndProcess<T>(built.GetCommandConfiguration(), d =>
            {
                rez = d;
                return false;
            },firstRowOnly:true);
            return rez;
        }
        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<T> GetSingleAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder,CancellationToken token)
        {
            builder.MustNotBeNull();
            var built = builder(db.GetSqlBuilder());
            T rez = default(T);
            await db.QueryAndProcessAsync<T>(built.GetCommandConfiguration(), d =>
            {
                rez = d;
                return false;
            },token,firstRowOnly:true).ConfigureAwait(false);
            return rez;
        }

        public static List<T> Query<T>(this DbConnection db, string sql, params object[] args) => db.Fetch<T>(c => c.Sql(sql, args));

        public static Task<List<T>> QueryAsync<T>(this DbConnection db,CancellationToken token ,string sql, params object[] args) => db.FetchAsync<T>(c => c.Sql(sql, args),token);


        /// <summary>
        /// It's Query with the sql built with a strongly typed builder
        /// </summary>
        /// <typeparam name="TProj"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static List<TProj> Query<TProj>(this DbConnection db,Func<IBuildQueryFrom, IGenerateSql<TProj>> builder)
        {
            builder.MustNotBeNull();
            var built = builder(db.GetSqlBuilder());
            var list=new List<TProj>();
            db.QueryAndProcess<TProj>(built.GetCommandConfiguration(), d =>
            {
                list.Add(d);
                return true;
            });
            return list;
        }
        
        /// <summary>
        /// It's Query with the sql built with a strongly typed builder
        /// </summary>
        /// <typeparam name="TProj"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static async Task<List<TProj>> QueryAsync<TProj>(this DbConnection db,Func<IBuildQueryFrom, IGenerateSql<TProj>> builder,CancellationToken token)
        {
            builder.MustNotBeNull();
            var built = builder(db.GetSqlBuilder());
            var list=new List<TProj>();
            await db.QueryAndProcessAsync<TProj>(built.GetCommandConfiguration(), d =>
            {
                list.Add(d);
                return true;
            },token).ConfigureAwait(false);
            return list;
        }       

        #endregion

        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, string sql, params object[] args)
        {
            return DbCommandExtensions.CreateAndSetupCommand(cnx, new CommandConfiguration(sql, args));
        }
    }
    
   
}