using System.Data.Common;
using SqlFu.Configuration.Internals;

namespace SqlFu.Providers
{
    public interface IDatabaseTools
    {
        bool TableExists(DbConnection cnx, string name, string schema = null);
    }
}