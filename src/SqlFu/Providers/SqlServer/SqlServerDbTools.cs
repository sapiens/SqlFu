using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerDbTools:IDatabaseTools
    {
       
        public bool TableExists(DbConnection cnx, TableName table)
        {
            var name = cnx.GetProvider().EscapeTableName(table);
            return cnx.GetValue<int?>(c => c.Sql($"select OBJECT_ID (N'{name}', N'U')")).HasValue;
        }

        public void DropTableIfExists(DbConnection db, TableName table)
        {
            throw new System.NotImplementedException();
        }
    }
}