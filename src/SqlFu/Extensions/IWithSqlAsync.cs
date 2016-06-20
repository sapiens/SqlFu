using System.Threading;

namespace SqlFu
{
    public interface IWithSqlAsync:IWithSql
    {
        CancellationToken Cancel { get; }
    }
}