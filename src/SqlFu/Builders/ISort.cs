using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISort<T> : IPaged<T>
    {
        ISort<T> OrderBy(Expression<Func<T, object>> column);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        ISort<T> OrderByIf(Func<bool> condition,Expression<Func<T, object>> column);
        ISort<T> OrderByDescending(Expression<Func<T, object>> column);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        ISort<T> OrderByDescendingIf(Func<bool> condition, Expression<Func<T, object>> column);
    }
}