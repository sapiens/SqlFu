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

        public static IHaveDbProvider GetProvider(DBType type)
        {
            switch (type)
            {
                case DBType.SqlServer: return new SqlServerProvider();
                case DBType.SqlServerCE: return new SqlServerCEProvider();
                case DBType.MySql: return new MySqlProvider();
                case DBType.PostgreSQL:return new PostgresProvider();
                case DBType.Oracle:return new OracleProvider();
                case DBType.SQLite:return new SqliteProvider();
            }
            throw new Exception("Unkown provider");
        }
    }
}