using System.Threading;
using System.Threading.Tasks;
using SqlFu;
using SqlFu.Builders;

namespace Tests._Fakes
{
    public class FakeSqlExecutor:IExecuteCustomSql
    {
        public CommandConfiguration Result { get; private set; }
        public int Execute(CommandConfiguration cfg)
        {
            Result = cfg;
            return 0;
        }

        public Task<int> ExecuteAsync(CommandConfiguration cfg, CancellationToken cancel)
        {
            throw new System.NotImplementedException();
        }
    }
}