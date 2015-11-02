using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface IWhere<T> : IGroup<T>
    {
        ISort<T> Where(Expression<Func<T, bool>> criteria);
    }

    public interface IHaving<T> : ISort<T>
    {
        ISort<T> Having(Expression<Func<T, bool>> criteria);

    }

    public interface IGroup<T> : IHaving<T>
    {
        IHaving<T> GroupBy(params Expression<Func<T, object>>[] columns);
    }
}