using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Executors;
using SqlFu.Providers;

namespace SqlFu
{
    /// <summary>
    /// It should be used for testing by libraries using SqlFu
    /// </summary>
    public class DbFactoryForTests:IDbFactory
    {
        private readonly string _cnx;
        private readonly SqlFuConfig _config;

        public DbFactoryForTests(IDbProvider provider,string cnx,SqlFuConfig config)
        {
            _cnx = cnx;
            _config = config;
            Provider = provider;            
        }
        public IDbProvider Provider { get; }
        public SqlFuConfig Configuration => _config;

        public DbConnection Create(DbConnection db = null)
        {
            SqlFuConnection cnx ;
            if (db != null)
            {
                cnx = new SqlFuConnection(Provider, db, _config);
            }
            else
            {
                cnx= new SqlFuConnection(Provider, _cnx, _config);                
            }
            cnx.Open();
            return cnx;

        }

        public DbConnection Create(string cnxString)
        {
            var db=new SqlFuConnection(Provider,cnxString??_cnx,_config);
            db.Open();
            return db;
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, DbConnection db = null)
        {
            return Task.FromResult(Create());
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, string cnxString)
        {
            return Task.FromResult(Create(cnxString));
        }
    }
}