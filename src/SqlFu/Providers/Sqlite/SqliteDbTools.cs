using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu.Providers.Sqlite
{
    public class SqliteDbTools:IDatabaseTools
    {
        private readonly IEscapeIdentifier _utils;

        public SqliteDbTools(IEscapeIdentifier utils)
        {
            _utils = utils;
        }

        public bool TableExists(DbConnection cnx, TableName table)
        {
            return cnx.GetValue<bool?>($"SELECT 1 FROM sqlite_master WHERE type='table' AND name=@0", table.Name)??false;
        }

        public void DropTableIfExists(DbConnection db, TableName table)
        {
            db.Execute($"drop table if exists {_utils.EscapeTableName(table)}");
        }
    }
}