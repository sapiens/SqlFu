using System;
using System.Data.Common;
using System.Linq.Expressions;

namespace SqlFu.Builders.Internals
{
    public class QueryBuilderCreator<T>:IProject<T>
    {
        private readonly DbConnection _cnx;

        public QueryBuilderCreator(DbConnection cnx)
        {
            _cnx = cnx;
        }

        public ISqlFrom<T, T> Select()
        {
            var q = new QueryBuilder<T, T>(new SqlQueryManager(_cnx.GetProvider(), SqlFuManager.Config.TableInfoFactory), _cnx);
            var parameter = Expression.Parameter(typeof(T));
            q.SelectColumns(Expression.Lambda(parameter,parameter));
            return q;
        }

        public ISqlFrom<T, TProjection> Select<TProjection>(Expression<Func<T, TProjection>> projection)
        {
            var q= new QueryBuilder<T, TProjection>(new SqlQueryManager(_cnx.GetProvider(),SqlFuManager.Config.TableInfoFactory), _cnx);
            q.SelectColumns(projection);
            return q;
        }

        public ISqlFrom<T, TProjection> Select<T1, TProjection>(Expression<Func<T, T1, TProjection>> projection)
        {
            var q = new QueryBuilder<T, TProjection>(new SqlQueryManager(_cnx.GetProvider(), SqlFuManager.Config.TableInfoFactory), _cnx);
            q.SelectColumns(projection);
            return q;
        }

        public ISqlFrom<T, TProjection> Select<T1, T2, TProjection>(Expression<Func<T, T1, T2, TProjection>> projection)
        {
            var q = new QueryBuilder<T, TProjection>(new SqlQueryManager(_cnx.GetProvider(), SqlFuManager.Config.TableInfoFactory), _cnx);
            q.SelectColumns(projection);
            return q;
        }
    }
}