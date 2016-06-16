using System;
using System.Data.Common;
using SqlFu.Builders;


namespace SqlFu
{
    public interface IWithSql
    {
        IProcessEachRow<T> WithSql<T>(Func<IBuildQueryFrom, IGenerateSql<T>> sql,Action<DbCommand> cfg=null);
        DbConnection Connection { get; }
     
    }
}