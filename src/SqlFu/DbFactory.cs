using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Configuration;
using SqlFu.Executors;
using SqlFu.Providers;

namespace SqlFu
{
    public class DbFactory : IDbFactory
    {
        private  IDbProvider _provider;
        private  string _connectionString;

        public DbFactory(SqlProfile profile)
        {
            Assign(profile);
        }

        protected DbFactory()
        {
            
        }

        internal void Assign(SqlProfile profile)
        {
            _provider = profile.Provider;
            _connectionString = profile.ConnectionString;
        }

        //public DbFactory(IDbProvider provider,string connectionString)
        //{
        //    _provider = provider;
        //    _connectionString = connectionString;
        //}

        public IDbProvider Provider => _provider;
        
        public DbConnection Create(DbConnection db=null)
        {
            if (db!=null) return new SqlFuConnection(db,_provider);
            return SqlFuManager.OpenConnection(_provider, _connectionString);           
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel)
        {
           return SqlFuManager.OpenConnectionAsync(_provider, _connectionString,cancel);
        }
    }

}