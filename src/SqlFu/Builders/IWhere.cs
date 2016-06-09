using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface IWhere<T> : IGroup<T>
    {
        IConnectWhere<T> Where(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectWhere<T> WhereIf(Func<bool> condition, Expression<Func<T, bool>> criteria);        
    }

    public interface IHaving<T> : ISort<T>
    {
        IConnectHaving<T> Having(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectHaving<T> HavingIf(Func<bool> condition, Expression<Func<T, bool>> criteria);

    }

    public interface IConnectHaving<T> : ISort<T>
    {
        IConnectHaving<T> And(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectHaving<T> AndIf(Func<bool> condition, Expression<Func<T, bool>> criteria);
        IConnectHaving<T> Or(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectHaving<T> OrIf(Func<bool> condition, Expression<Func<T, bool>> criteria);
    }

    public interface IConnectWhere<T>:IGroup<T>
    {
        IConnectWhere<T> And(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectWhere<T> AndIf(Func<bool> condition, Expression<Func<T, bool>> criteria);
        IConnectWhere<T> Or(Expression<Func<T, bool>> criteria);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        IConnectWhere<T> OrIf(Func<bool> condition, Expression<Func<T, bool>> criteria);
    }

    public interface IGroup<T> : IHaving<T>
    {
        IHaving<T> GroupBy(params Expression<Func<T, object>>[] columns);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        IHaving<T> GroupByIf(Func<bool> condition, params Expression<Func<T, object>>[] columns);
    }
}