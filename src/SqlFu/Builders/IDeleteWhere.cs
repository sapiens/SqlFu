using System;
using System.Linq.Expressions;

namespace SqlFu.Builders
{
    public interface IDeleteWhere<T>:IBuildSql
    {
        IDeleteWhereConnector<T> Where(Expression<Func<T, bool>> criteria);
        IDeleteFromSubQuery<T> Where(Expression<Func<T, object>> column);
    }
}