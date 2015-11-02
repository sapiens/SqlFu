using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface ISelect<T>
    {
        IGenerateSql<T> AllColumns(bool distinct=false);
        IGenerateSql<TProj> Select<TProj>(Expression<Func<T, TProj>> selector,bool distinct=false);

    }
}