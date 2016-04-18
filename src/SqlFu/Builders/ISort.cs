using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISort<T> : IPaged<T>
    {
        ISort<T> OrderBy(Expression<Func<T, object>> column);
        ISort<T> OrderByDescending(Expression<Func<T, object>> column);
    }
}