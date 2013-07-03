using System;
using System.Data.Common;
using SqlFu.Providers;
using SqlFu.Providers.SqlServer;

namespace SqlFu
{
    public static class ProviderFactory
    {
        public static IHaveDbProvider GetProviderByName(string providerName)
        {
            switch (providerName)
            {
                case SqlServerProvider.ProviderName:
                    return new SqlServerProvider();
                case MySqlProvider.ProviderName:
                    return new MySqlProvider();
                case PostgresProvider.ProviderName:
                    return new PostgresProvider();
                    //case OracleProvider.ProviderName:
                    //    return new OracleProvider();
                case SqlServerCEProvider.ProviderName:
                    return new SqlServerCEProvider();
                case SqliteProvider.ProviderName:
                    return new SqliteProvider();
            }
            throw new Exception("Unkown provider");
        }

        public static IHaveDbProvider GetProvider(this DbConnection cnx)
        {
            var sqlfu = cnx as SqlFuConnection;
            if (sqlfu != null)
            {
                return sqlfu.Provider;
            }
            var type = cnx.GetType().Name;

            if (type.Equals("SqlConnection", StringComparison.InvariantCultureIgnoreCase))
            {
                return new SqlServerProvider();
            }

            if (type.StartsWith("MySql"))
            {
                return new MySqlProvider();
            }

            if (type.StartsWith("Npgsql"))
            {
                return new PostgresProvider();
            }

            if (type.StartsWith("SQLite"))
            {
                return new SqliteProvider();
            }

            if (type.Equals("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase))
            {
                return new SqlServerCEProvider();
            }

            throw new NotSupportedException();
        }

        public static IHaveDbProvider GetProvider(DbEngine engine)
        {
            switch (engine)
            {
                case DbEngine.SqlServer:
                    return new SqlServerProvider();
                case DbEngine.SqlServerCE:
                    return new SqlServerCEProvider();
                case DbEngine.MySql:
                    return new MySqlProvider();
                case DbEngine.PostgreSQL:
                    return new PostgresProvider();
                    //case DbEngine.Oracle:
                    //    return new OracleProvider();
                case DbEngine.SQLite:
                    return new SqliteProvider();
            }
            throw new Exception("Unkown provider");
        }
    }
}