using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using CavemanTools;
using SqlFu.Builders;

namespace SqlFu
{
    /// <summary>
    /// Used to issue quick queries
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryOver<T>
    {
        private readonly IDbFactory _fact;

        public QueryOver(IDbFactory fact)
        {
            _fact = fact;

        }


        public IQuerySql<TResult> Build<TResult>(Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> criteria = null,CancellationToken? cancel=null)
        {
            var db = _fact.Create();
            criteria = criteria ?? (x => true);
            var b = db.GetSqlBuilder().From<T>().Where(criteria).Select(selector);
            var r = new FluentCommandExecutor<TResult>(db, b, Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }

        public IQuerySql<T> Build(Expression<Func<T, bool>> criteria = null, CancellationToken? cancel = null)
        {
            var db = _fact.Create();
            criteria = criteria ?? (x => true);
            var b = db.GetSqlBuilder().From<T>().Where(criteria).SelectAll(useAsterisk:true);
            var r = new FluentCommandExecutor<T>(db, b, Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }
        
        public IQuerySql<TResult> Build<TResult>(Func<IWhere<T>, IGenerateSql<TResult>> sqlBuilder, CancellationToken? cancel = null)
        {
            var db = _fact.Create();
            var b = db.GetSqlBuilder().From<T>();
            var r = new FluentCommandExecutor<TResult>(db,sqlBuilder(b), Empty.ActionOf<DbCommand>(), true,cancel);
            return r;
        }
    }
}