using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteCheckWriter:AbstractCheckWriter
    {
        public SqliteCheckWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            SqliteDDLWriter.WriteColumnsNames(columns,builder,SqliteProvider.EscapeIdentifier);
        }
    }
}