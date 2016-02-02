using System;
using System.Data.Common;
using CavemanTools.Logging;
using SqlFu.Providers.SqlServer;

namespace SqlFu
{
    public abstract class DbTest : IDisposable
    {
        protected DbConnection _db;
        public static string Connex = @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=True";
        
        public static DbConnection GetConnection()
        {
            LogManager.OutputToTrace();
           
            return SqlFuManager.OpenConnection(SqlServer2012Provider.Instance, Connex);
        }

        protected DbTest()
        {
            _db =GetConnection();
        }
        public virtual void Dispose()
        {
            _db.Dispose();
        }
    }
}