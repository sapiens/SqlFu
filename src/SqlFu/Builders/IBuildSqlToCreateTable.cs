using SqlFu.Configuration.Internals;

namespace SqlFu.Builders
{
    public interface IBuildSqlToCreateTable
    {
        string GetTableCreateSql(TableInfo info);
    }
}