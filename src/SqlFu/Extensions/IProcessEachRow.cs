using System;

namespace SqlFu
{
    public interface IProcessEachRow<T>:IQuerySql<T>
    {
        IQueryAndProcess ProcessEachRow(Func<T, bool> processor);
    }
}