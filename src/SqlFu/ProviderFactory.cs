using System;
using SqlFu.Providers;

namespace SqlFu
{
    internal static class ProviderFactory
    {
        public static IHaveDbProvider GetProviderByName(string providerName)
        {
            switch (providerName)
            {
                case SqlServerProvider.ProviderName:return new SqlServerProvider();
                case MySqlProvider.ProviderName:return new MySqlProvider();
                case PostgresProvider.ProviderName:return new PostgresProvider();
                case OracleProvider.ProviderName:return new OracleProvider();
                case SqlServerCEProvider.ProviderName:return new SqlServerCEProvider();
                case SqliteProvider.ProviderName:return new SqliteProvider();
            }
            throw new Exception("Unkown provider");
        }

        public static IHaveDbProvider GetProvider(DbEngine engine)
        {
            switch (engine)
            {
                case DbEngine.SqlServer: return new SqlServerProvider();
                case DbEngine.SqlServerCE: return new SqlServerCEProvider();
                case DbEngine.MySql: return new MySqlProvider();
                case DbEngine.PostgreSQL:return new PostgresProvider();
                case DbEngine.Oracle:return new OracleProvider();
                case DbEngine.SQLite:return new SqliteProvider();
            }
            throw new Exception("Unkown provider");
        }
    }
}