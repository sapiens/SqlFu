using System;

namespace SqlFu.Builders
{
    public interface IPaged<T>:ISelect<T>
    {
        ISelect<T> Limit(int take, long offset=0);
        /// <summary>
        /// Generate the sql only if condition returns true.
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="take"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        ISelect<T> LimitIf(Func<bool> condition,int take, long offset=0);
    }
}