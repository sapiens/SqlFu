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
        #region Internal
        internal static int Execute(this DbConnection cnx, CommandConfiguration cfg)
        {
            using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                return cmd.Execute();
            }
        }

        internal static async Task<int> ExecuteAsync(this DbConnection cnx, CommandConfiguration cfg, CancellationToken token)
        {
            using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                return await cmd.ExecuteAsync(token).ConfigureAwait(false);
            }
        }

        
        internal static T GetValue<T>(this DbConnection db, CommandConfiguration cfg)
        {
            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                return cmd.GetValue<T>();
            }
            
        }

        internal static async Task<T> GetValueAsync<T>(this DbConnection db, CommandConfiguration cfg, CancellationToken token)
        {
            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                return await cmd.GetValueAsync<T>(token).ConfigureAwait(false);
            }
        }

        internal static void QueryAndProcess<T>(this DbConnection cnx, CommandConfiguration cfg, Func<T, bool> processor, T anonModel = default(T),
          bool firstRowOnly = false)
        {
            using (var cmd = cnx.CreateAndSetupCommand(cfg))
            {
                cmd.QueryAndProcess(processor, firstRowOnly: firstRowOnly);
            }
        }

        internal static async Task QueryAndProcessAsync<T>(this DbConnection db, CommandConfiguration cfg, Func<T, bool> processor, CancellationToken token, T anonModel = default(T),
          bool firstRowOnly = false)
        {

            using (var cmd = db.CreateAndSetupCommand(cfg))
            {
                await cmd.QueryAndProcessAsync(token, processor, firstRowOnly: firstRowOnly).ConfigureAwait(false);
            }
        }


        #endregion

        #region Execute

        /// <summary>
        /// Checks if any of provided db objects (table, view) exists
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="items"></param>
        public static void ExecuteCreation(this DbConnection cnx, IEnumerable<ICreateDbItem> items)
        {
            using (var t = cnx.BeginTransaction())
            {
                foreach (var item in items)
                {
                    cnx.AddDbObjectOrIgnore($"create table {cnx.Provider().EscapeTableName(item.Name)} {item.Sql}");
                }
                t.Commit();
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
        #endregion

        #region  GetValue

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

       

        public static Task<T> GetValueAsync<T>(this DbConnection db, Action<IConfigureCommand> cfg, CancellationToken token)
        {
            var cmd=new CommandConfiguration();
            cfg(cmd);
            return GetValueAsync<T>(db,cmd, token);
        }

        /// <summary>
        /// Returns a single value from the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static T QueryValue<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder)
        {
            return db.GetValue<T>(builder(db.GetSqlBuilder()).GetCommandConfiguration());
        }

        /// <summary>
        ///  Returns a single value from the query.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<T> QueryValueAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder, CancellationToken token)
        {
            return db.GetValueAsync<T>(builder(db.GetSqlBuilder()).GetCommandConfiguration(), token);
        }


        #endregion

        #region Get row
        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="cfg"></param>
        /// <param name="anonModel">Anonymous type</param>
        /// <returns></returns>
        public static T GetRow<T>(this DbConnection db, Action<IConfigureCommand> cfg, T anonModel = null) where T : class
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
        public static async Task<T> GetRowAsync<T>(this DbConnection db, Action<IConfigureCommand> cfg, CancellationToken token, T anonModel = null) where T : class
        {
            T rez = null;
            await db.QueryAndProcessAsync(cfg, d =>
           {
               rez = d;
               return false;
           }, token, anonModel,true);
            return rez;
        }


        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>    
        /// <returns></returns>
        public static T QueryRow<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder)
        {
            builder.MustNotBeNull();
            return db.WithSql(builder).GetFirstRow();
        }
        /// <summary>
        /// Gets a single row then maps it to a poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static  Task<T> QueryRowAsync<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> builder, CancellationToken token)
        {
            builder.MustNotBeNull();
            return db.WithSql(builder, cancel: token).GetFirstRowAsync();            
        }


        public static T GetSingleRow<T>(this DbConnection db, Expression<Func<T, bool>> criteria = null, Expression<Func<T, object>> orderBy = null)
        {

            return db.QueryRow(d =>

                d.From<T>().Where(criteria ?? (f => true))
                    .OrderBy(orderBy ?? (f => 1))
                    .Limit(1)
                    .SelectAll()
                );
        }

        public static Task<T> GetSingleRowAsync<T>(this DbConnection db, CancellationToken cancel, Expression<Func<T, bool>> criteria = null, Expression<Func<T, object>> orderBy = null)
        {
            return db.QueryRowAsync(d =>

                d.From<T>().Where(criteria ?? (f => 1 == 1))
                    .OrderBy(orderBy ?? (f => 1))
                    .Limit(1)
                    .SelectAll()
                , cancel);

        }


        #endregion

        #region Fetch

        /// <summary>
        /// Runs a query and returns the results as a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg"></param>
        /// <param name="anonModel"></param>
        /// <returns></returns>
        public static List<T> Fetch<T>(this DbConnection cnx, Action<IConfigureCommand> cfg, T anonModel = default(T))
        {
            var list=new List<T>();
            cnx.QueryAndProcess(cfg,d=> { list.Add(d);
                                            return true;
            },anonModel);
            return list;
        }

        /// <summary>
        /// Runs a query and returns the results as a List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg"></param>
        /// <param name="anonModel"></param>
        /// <returns></returns>
        public static async Task<List<T>> FetchAsync<T>(this DbConnection cnx, Action<IConfigureCommand> cfg,CancellationToken token ,T anonModel = default(T))
        {
            var list=new List<T>();
            await cnx.QueryAndProcessAsync(cfg,d=> { list.Add(d);
                                            return true;
            },token,anonModel);
            return list;
        }


        /// <summary>
        /// Synonym for Fetch.For now, it exists for compatibility reasons
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>     
        public static List<T> Query<T>(this DbConnection db, string sql, params object[] args) => db.Fetch<T>(c => c.Sql(sql, args));
        /// <summary>
        /// Synonym for FetchAsync.For now, it exists for compatibility reasons
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="token"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Task<List<T>> QueryAsync<T>(this DbConnection db, CancellationToken token, string sql, params object[] args) => db.FetchAsync<T>(c => c.Sql(sql, args), token);



        /// <summary>
        /// It's Fetch with a strongly typed builder
        /// </summary>
        /// <typeparam name="TProj"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static List<TProj> QueryAs<TProj>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<TProj>> builder)
        {
            return db.WithSql(builder).GetRows();
        }

        /// <summary>
        /// It's Fetch with a strongly typed builder
        /// </summary>
        /// <typeparam name="TProj"></typeparam>
        /// <param name="db"></param>
        /// <param name="builder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<List<TProj>> QueryAsAsync<TProj>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<TProj>> builder, CancellationToken token)
        {
            return db.WithSql(builder, cancel: token).GetRowsAsync();          
        }

        #endregion

        #region QueryAndProcess
        /// <summary>
        /// Processes each row (as a strongly typed poco) as it is read.
        /// Used to process a big stream of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg">Sql and command arguments</param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="anonModel">Map to anonymous</param>
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

        /// <summary>
        /// Processes each row (as a strongly typed poco) as it is read.
        /// Used to process a big stream of results.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="sqlBuilder"></param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="firstRowOnly"></param>
        public static void QueryAndProcess<T>(this DbConnection cnx, Func<IBuildQueryFrom, IGenerateSql<T>> sqlBuilder,
            Func<T, bool> processor, bool firstRowOnly = false)
        {
            cnx.QueryAndProcess(sqlBuilder(cnx.GetSqlBuilder()).GetCommandConfiguration(), processor, default(T), firstRowOnly);
        }

        /// <summary>
        /// Processes each row (as a strongly typed poco) as it is read.
        /// Used to process a big stream of results. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="cfg">Sql and command arguments</param>
        /// <param name="processor">True/False to continue processing</param>
        /// <param name="token"></param>
        /// <param name="anonModel">Map to anonymous</param>
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
        /// Processes each row (as a strongly typed poco) as it is read.
        /// Used to process a big stream of results. 
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


        #endregion

        #region Paged queries

        public static PagedResult<T> FetchPaged<T>(this DbConnection db,Action<IConfigureCommand> cfg,Pagination page,T anonModel=null) where T:class
        {
            typeof(T).Must(t=>t!=typeof(object),"Dynamic types are not supported");
            var cmd=new CommandConfiguration();
            cfg(cmd);
            var builder = new PagedQueryBuilder(db.GetPocoInfo<T>(), db.Provider());
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
            var builder = new PagedQueryBuilder(db.GetPocoInfo<T>(), db.Provider());
            var data = builder.Build(cmd.SqlText, cmd.Args, page);

            var result=new PagedResult<T>();
            result.LongCount = await 
                db.GetValueAsync<long>(c => c.Sql(data.CountSql, cmd.Args).WithCommandOptions(cmd.ApplyOptions),token).ConfigureAwait(false);
            if (result.Count == 0) return result;

            result.Items = await db.FetchAsync<T>(c => c.Sql(data.PagedSql, data.Args).WithCommandOptions(cmd.ApplyOptions),token).ConfigureAwait(false);
            return result;
        }
        #endregion

        /// <summary>
        /// Used to build a query string using expressions
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static IBuildQueryFrom GetSqlBuilder(this DbConnection db) => new SqlFrom(db.Provider(), db.SqlFuConfig().TableInfoFactory);

        /// <summary>
        /// Generates sql from strongly typed query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="sqlBuilder"></param>
        /// <returns>Configuration object containing generated sql and arguments</returns>
        public static CommandConfiguration BuildSql<T>(this DbConnection db,
            Func<IBuildQueryFrom, IGenerateSql<T>> sqlBuilder)
            => sqlBuilder(db.GetSqlBuilder()).GetCommandConfiguration();

        /// <summary>
        /// Returns the ready to be used (escaped) inside a query table name for that poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <returns></returns>
        public static string GetTableName<T>(this DbConnection cnx) => SqlFuManager.Config.TableInfoFactory.GetInfo(typeof(T)).EscapeName(cnx.Provider());


        /// <summary>
        /// To be used by custom helpers
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static DbCommand CreateAndSetupCommand(this DbConnection cnx, string sql, params object[] args) => 
            cnx.CreateAndSetupCommand(new CommandConfiguration(sql, args));

        /// <summary>
        /// Used to execute add views/sproc statements and ignore duplication messages
        /// </summary>
        /// <param name="db"></param>
        /// <param name="addObjectStatement"></param>       
        public static void AddDbObjectOrIgnore(this DbConnection db, string addObjectStatement)
        {
            try
            {
                db.Execute(addObjectStatement);
            }
            catch (DbException x) when (db.Provider().ObjectExists(x))
            {
                //already exists, move on
            }            
        }

        /// <summary>
        /// Provides a fluent builder to specify sql, configure and execute the command 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db">Connection</param>
        /// <param name="sqlBuilder">Sql generator</param>
        /// <param name="cfg">Command configuration</param>
        /// <param name="cancel">Cancellation token for async operations</param>
        /// <returns></returns>
        public static IProcessEachRow<T> WithSql<T>(this DbConnection db, Func<IBuildQueryFrom, IGenerateSql<T>> sqlBuilder,
            Action<DbCommand> cfg = null,CancellationToken? cancel=null)
        =>new FluentCommandBuilder<T>(db,sqlBuilder(db.GetSqlBuilder()),cfg,cancel);
    }
}