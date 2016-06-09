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
        /// <param name="useAsterisk">Literally use * instead of naming each column</param>
        /// <returns></returns>
        IGenerateSql<T> SelectAll(bool distinct=false,bool useAsterisk=false);
        IGenerateSql<TProj> Select<TProj>(Expression<Func<T, TProj>> selector,bool distinct=false);

    }

  
}