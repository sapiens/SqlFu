using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SqlFu.Builders
{
    public class CustomSqlExecutor:IExecuteCustomSql
    {
        private readonly DbConnection _db;
       
        public CustomSqlExecutor(DbConnection db)
        {
            _db = db;
           
        }


        public int Execute(CommandConfiguration cfg) => _db.Execute(cfg);

        public Task<int> ExecuteAsync(CommandConfiguration cfg, CancellationToken? cancel=null)
            => _db.ExecuteAsync(cfg, cancel??CancellationToken.None);
    }
}