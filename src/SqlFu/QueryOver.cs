using System;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using CavemanTools;
using SqlFu.Builders;
using SqlFu.Configuration;

namespace SqlFu
{
    /// <summary>
    /// Used to issue quick queries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryOver<T> where T:class
    {
        private readonly Func<DbConnection> _fact;
        private readonly TableName _name;

        public QueryOver(Func<DbConnection> fact,TableName name=null)
        {
            _fact = fact;
            _name = name;
         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="selector">Choose column(s). If columns then return an anonymous object</param>
        /// <param name="criteria"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public IQuerySql<TResult> Select<TResult>(Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> criteria = null,CancellationToken? cancel=null)
        {
            var db = GetDb();
            criteria = criteria ?? (x => true);
            var b = db.GetSqlBuilder().From<T>().Where(criteria).Select(selector);
            var r = new FluentCommandExecutor<TResult>(db, b, Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }

        private DbConnection GetDb()
        {
            var db = _fact();
            if (_name != null) db.TableNameFor<T>(_name);
          db.Open();
            return db;
        }

        public IQuerySql<T> SelectAllColumns(Expression<Func<T, bool>> criteria = null, CancellationToken? cancel = null)
        {
            var db = GetDb();
            criteria = criteria ?? (x => true);
            var b = db.GetSqlBuilder().From<T>().Where(criteria).SelectAll(useAsterisk:true);
            var r = new FluentCommandExecutor<T>(db, b, Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }
        
        /// <summary>
        /// Builder for a more complex query
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="sqlBuilder"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        public IQuerySql<TResult> Build<TResult>(Func<IWhere<T>, IGenerateSql<TResult>> sqlBuilder, CancellationToken? cancel = null)
        {
            var db = GetDb();
            var b = db.GetSqlBuilder().From<T>();
            var r = new FluentCommandExecutor<TResult>(db,sqlBuilder(b), Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }
    }
}