using System.Threading;
using System.Threading.Tasks;

namespace SqlFu.Builders
{
    public interface IExecuteCustomSql
    {
        int Execute(CommandConfiguration cfg);
        Task<int> ExecuteAsync(CommandConfiguration cfg, CancellationToken? cancel=null);
    }
}