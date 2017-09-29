using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Executors;
using SqlFu.Providers;

namespace SqlFu
{
    /// <summary>
    /// It should be used when testing by libraries using SqlFu
    /// </summary>
    public class DbFactoryForTest:IDbFactory
    {
        private readonly string _cnx;
        private readonly SqlFuConfig _config;

        public DbFactoryForTest(IDbProvider provider,string cnx,SqlFuConfig config)
        {
            _cnx = cnx;
            _config = config;
            Provider = provider;
        }
        public IDbProvider Provider { get; }
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
            throw new NotImplementedException();
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, DbConnection db = null)
        {
            return Task.FromResult(Create());
        }

        public Task<DbConnection> CreateAsync(CancellationToken cancel, string cnxString)
        {
            throw new NotImplementedException();
        }
    }
}