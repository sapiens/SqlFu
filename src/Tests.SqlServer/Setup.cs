using System;
using System.Data.Common;
using SqlFu;
using SqlFu.Providers.SqlServer;
using System.Data.SqlClient;

using CavemanTools;
using CavemanTools.Logging;
using CavemanTools.Model.ValueObjects;


namespace Tests.SqlServer
{
    public class Setup
    {
        public const string Connex = @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True;Asynchronous Processing=True";

        static Setup()
        {
            LogManager.OutputTo(s=>System.Diagnostics.Debug.WriteLine(s));
            SqlFuManager.Configure(c =>
            {
                
                c.AddProfile(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),Connex);              
              //  c.AddProfile(new SqliteProvider(SQLiteFactory.Instance.CreateConnection),"Data Source = mydb.db; Version = 3;","sqlite");              
            });
        }

        public static DbConnection GetConnection() => SqlFuManager.GetDbFactory(
//#if SqlServer
//#elif Sqlite
//            "sqlite"
//#endif
            ).Create();


    }
}