using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface IProject<T>
    {
        ISqlFrom<T, T> Select();        
        ISqlFrom<T, TProjection> Select<TProjection>(Expression<Func<T, TProjection>> projection);        
        ISqlFrom<T, TProjection> Select<T1,TProjection>(Expression<Func<T,T1, TProjection>> projection);        
        ISqlFrom<T, TProjection> Select<T1,T2,TProjection>(Expression<Func<T,T1,T2, TProjection>> projection);        
    }
}