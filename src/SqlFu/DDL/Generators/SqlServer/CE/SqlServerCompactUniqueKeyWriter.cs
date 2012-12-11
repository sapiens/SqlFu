using System.Text;

namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactUniqueKeyWriter : SqlServerUniqueKeyWriter
    {
        public SqlServerCompactUniqueKeyWriter(StringBuilder builder) : base(builder, DbEngine.SqlServerCE)
        {
        }

        protected override string GetOptionsBeforeColumns(DbEngineOptions options)
        {
            return "";
        }
    }
}