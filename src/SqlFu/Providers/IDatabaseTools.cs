using System.Data.Common;

namespace SqlFu.Providers
{
    public interface IDatabaseTools
    {
        bool TableExists(DbConnection cnx, TableName table);
    }
}