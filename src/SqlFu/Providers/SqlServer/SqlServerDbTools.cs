using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerDbTools:IDatabaseTools
    {
        private readonly IEscapeIdentifier _utils;

        public SqlServerDbTools(IEscapeIdentifier utils)
        {
            _utils = utils;
        }

        public bool TableExists(DbConnection cnx, TableName table)
        {
            var name = _utils.EscapeTableName(table);
            return cnx.GetValue<int?>(c => c.Sql($"select OBJECT_ID (N'{name}', N'U')")).HasValue;
        }

        public void DropTableIfExists(DbConnection db, TableName table)
        {
            var name = _utils.EscapeTableName(table);
            db.Execute($"IF OBJECT_ID('{name}', 'U') IS NOT NULL DROP TABLE {name}");
        }
    }
}