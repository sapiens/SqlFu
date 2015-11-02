using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISqlFrom<T, TProjection>:ISqlWhere<T,TProjection>
    {
        ISqlFrom<T, TProjection> Distinct();
        ISqlFrom<T, TProjection> InnerJoin<TOther>(Expression<Func<T, TOther, bool>> criteria);
        ISqlFrom<T, TProjection> LeftJoin<TOther>(Expression<Func<T, TOther, bool>> criteria);
    }
}