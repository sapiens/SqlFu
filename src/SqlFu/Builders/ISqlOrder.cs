using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISqlOrder<T, TProjection>:ISqlLimit<TProjection>
    {
        ISqlOrder<T, TProjection> OrderBy(Expression<Func<TProjection, object>> criteria);
        ISqlOrder<T, TProjection> OrderBy<TOther>(Expression<Func<TOther, object>> criteria);
        ISqlOrder<T, TProjection> OrderByDescending(Expression<Func<TProjection, object>> criteria);
        ISqlOrder<T, TProjection> OrderByDescending<TOther>(Expression<Func<TOther, object>> criteria);
    }
}