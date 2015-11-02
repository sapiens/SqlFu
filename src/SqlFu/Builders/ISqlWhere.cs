using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISqlWhere<T, TProjection>:ISqlGroup<T,TProjection>
    {
        ISqlWhereConnectors<T, TProjection> Where(Expression<Func<T, bool>> criteria);
        ISqlWhereConnectors<T, TProjection> Where<TOther>(Expression<Func<TOther, bool>> criteria);
        ISqlWhereOptions<T, TProjection> Where(Expression<Func<T, object>> column);
        ISqlWhereConnectors<T, TProjection> WhereExists(IBuildSql subquery);
        ISqlWhereConnectors<T, TProjection> WhereNotExists(IBuildSql subquery);        
    }
}