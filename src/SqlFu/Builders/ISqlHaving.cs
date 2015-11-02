using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISqlHaving<T, TProjection>:ISqlOrder<T,TProjection>
    {
        ISqlHavingConnectors<T, TProjection> Having(Expression<Func<T, bool>> criteria);
        ISqlHavingConnectors<T, TProjection> Having<TOther>(Expression<Func<TOther, bool>> criteria);
    }
}