using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu.Providers
{
    public interface IDatabaseTools
    {
        bool TableExists(DbConnection cnx, TableName table);
    }
}