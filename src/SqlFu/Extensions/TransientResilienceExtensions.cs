using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model.Persistence;
using SqlFu.Builders;

namespace SqlFu
{
    public static class TransientResilienceExtensions
    {

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbFactory"></param>
        /// <param name="action"></param>
        /// <param name="tryCount"></param>
        /// <param name="wait">How much to wait in miliseconds before retrying</param>
        /// <exception cref="DbException"></exception>
        /// <returns></returns>
        [Obsolete("Use RetryOnTransientError")]
        public static T HandleTransientErrors<T>(this IDbFactory dbFactory,Func<DbConnection,T> action,int tryCount=10,int wait=100)
        {
           return dbFactory.RetryOnTransientError(d => action(d.Connection));
        }

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="tryCount"></param>
        /// <param name="wait">How much to wait in miliseconds before retrying</param>
        /// <exception cref="DbException"></exception>
        /// <param name="action"></param>
        [Obsolete("Use RetryOnTransientError")]
        public static void HandleTransientErrors(this IDbFactory dbFactory, Action<DbConnection> action, int tryCount = 10, int wait = 100)
        {
            dbFactory.RetryOnTransientError(d => action(d.Connection));
        }

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbFactory"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <param name="tryCount"></param>
        /// <param name="wait">How much to wait in miliseconds before retrying</param>
        /// <exception cref="DbException"></exception>
        /// <returns></returns>
        [Obsolete("Use RetryOnTransientErrorAsync")]
        public static Task<T> HandleTransientErrorsAsync<T>(this IDbFactory dbFactory, CancellationToken token,Func<DbConnection,CancellationToken,Task<T>> action, int tryCount = 10, int wait = 100)
        {

            return dbFactory.RetryOnTransientErrorAsync(token, d => action(d.Connection, token));          
        }



        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="dbFactory"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        /// <param name="tryCount"></param>
        /// <param name="wait">How much to wait in miliseconds before retrying</param>
        /// <exception cref="DbException"></exception>
        /// <returns></returns>
        [Obsolete("Use RetryOnTransientErrorAsync")]
        public static  Task HandleTransientErrorsAsync(this IDbFactory dbFactory, CancellationToken token,Func<DbConnection,CancellationToken,Task> action, int tryCount = 10, int wait = 100)
        {

            return dbFactory.RetryOnTransientErrorAsync(token, d => action(d.Connection, token));
          
        }

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        public static Task RetryOnTransientErrorAsync(this IDbFactory factory,CancellationToken token,Func<IWithSqlAsync,Task> action)
        {
            return ModelTools.RetryOnException<DbException>(async () =>
            {
                using (var db = await factory.CreateAsync(token).ConfigureAwait(false))
                {
                    var op=new ResilientWithSql(db,token);
                    await action(op).ConfigureAwait(false);
                }
            }, x =>
            {
                if (factory.Provider.IsDbBusy(x)) return OnExceptionAction.IgnoreAndContinue;
                return OnExceptionAction.Throw;

            }, SqlFuManager.Config.TransientErrors.Tries, SqlFuManager.Config.TransientErrors.Wait);

        }

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="action"></param>
        public static void RetryOnTransientError(this IDbFactory factory,Action<IWithSql> action)
        {
            ModelTools.RetryOnException<DbException>(() =>
            {
                using (var db = factory.Create())
                {
                    var op = new ResilientWithSql(db, CancellationToken.None);
                    action(op);
                }
            }, x =>
            {
                if (factory.Provider.IsDbBusy(x)) return OnExceptionAction.IgnoreAndContinue;
                return OnExceptionAction.Throw;

            },SqlFuManager.Config.TransientErrors.Tries,SqlFuManager.Config.TransientErrors.Wait);
        }

        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="action"></param>
        public static T RetryOnTransientError<T>(this IDbFactory factory,Func<IWithSql,T> action)
        {
            T result = default(T);
            ModelTools.RetryOnException<DbException>(() =>
            {
                using (var db = factory.Create())
                {
                    var op = new ResilientWithSql(db, CancellationToken.None);
                    result=action(op);
                }
            }, x =>
            {
                if (factory.Provider.IsDbBusy(x)) return OnExceptionAction.IgnoreAndContinue;
                return OnExceptionAction.Throw;

            },SqlFuManager.Config.TransientErrors.Tries,SqlFuManager.Config.TransientErrors.Wait);
            return result;
        }


        /// <summary>
        /// Wraps db access action and retries it for 10 times with a slight delay between retries
        /// if the db is temporary unavailable (timeout or connection limit reached)
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="token"></param>
        /// <param name="action"></param>
        public static async Task<T> RetryOnTransientErrorAsync<T>(this IDbFactory factory, CancellationToken token, Func<IWithSqlAsync, Task<T>> action)
        {
            T result = default(T);
            await ModelTools.RetryOnException<DbException>(async () =>
            {
                using (var db = await factory.CreateAsync(token).ConfigureAwait(false))
                {
                    var op = new ResilientWithSql(db, token);
                    result=await action(op).ConfigureAwait(false);
                }
            }, x =>
            {
                if (factory.Provider.IsDbBusy(x)) return OnExceptionAction.IgnoreAndContinue;
                return OnExceptionAction.Throw;

            }, SqlFuManager.Config.TransientErrors.Tries, SqlFuManager.Config.TransientErrors.Wait);
            return result;
        }

        class ResilientWithSql : IWithSqlAsync
        {
            
            public ResilientWithSql(DbConnection db,CancellationToken cancel)
            {
                Connection = db;
                Cancel = cancel;
            }

            public IProcessEachRow<T> WithSql<T>(Func<IBuildQueryFrom, IGenerateSql<T>> sql,
                Action<DbCommand> cfg = null)
                => Connection.WithSql(sql, cfg, Cancel);

            public DbConnection Connection { get; }
       
            public CancellationToken Cancel { get; }
        }
    }
}