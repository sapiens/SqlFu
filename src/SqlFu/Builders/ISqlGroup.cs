using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISqlGroup<T, TProjection>:ISqlHaving<T,TProjection>
    {
        ISqlGroup<T, TProjection> GroupBy(params Expression<Func<TProjection, object>>[] expression);
    }
}