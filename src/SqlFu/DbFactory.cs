using System;
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

        public DbFactory(DbAccessProfile profile, SqlFuConfig config)
        {
            Assign(profile,config);
        }

        protected DbFactory()
        {
            
        }

        internal void Assign(DbAccessProfile profile,SqlFuConfig config)
        {
            _provider = profile.Provider;
            _connectionString = profile.ConnectionString;
            Configuration = config;
        }

       public IDbProvider Provider => _provider;
        public SqlFuConfig Configuration { get; private set; }

        public DbConnection Create(DbConnection db=null)
        {
            if (db == null) return SqlFuManager.OpenConnection(_provider, _connectionString);
            if (db is SqlFuConnection) return db;
            return new SqlFuConnection(_provider, db,SqlFuManager.Config);
        }

        public DbConnection Create(string cnxString)
        {
            return SqlFuManager.OpenConnection(_provider, cnxString??_connectionString);
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, DbConnection db = null)
        {
            if (db != null) return Task.FromResult((DbConnection)new SqlFuConnection(_provider, db, SqlFuManager.Config));
            return SqlFuManager.OpenConnectionAsync(_provider, _connectionString,cancel);
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, string cnxString)
        {
            return SqlFuManager.OpenConnectionAsync(_provider, cnxString ?? _connectionString, cancel);
        }
    }

}