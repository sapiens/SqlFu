using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISelect<T>
    {
        /// <summary>
        /// select * 
        /// </summary>
        /// <param name="distinct"></param>
        /// <returns></returns>
        IGenerateSql<T> SelectAll(bool distinct=false);
        IGenerateSql<TProj> Select<TProj>(Expression<Func<T, TProj>> selector,bool distinct=false);

    }

  
}