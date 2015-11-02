using System.Data.Common;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerDbTools:IDatabaseTools
    {
        public bool TableExists(DbConnection cnx, string name, string schema = null)
        {
            var table = cnx.GetProvider().EscapeTableName(name, schema);

            return cnx.GetValue<int?>(c => c.Sql($"select OBJECT_ID (N'{table}', N'U')")).HasValue;

            

        }
    }
}