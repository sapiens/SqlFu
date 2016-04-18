using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu.Providers
{
    public interface IDatabaseTools
    {
        bool TableExists(DbConnection cnx, TableName table);
        void DropTableIfExists(DbConnection db, TableName table);
    }
}