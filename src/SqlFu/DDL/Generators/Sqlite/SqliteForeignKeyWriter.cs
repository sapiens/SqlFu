using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteForeignKeyWriter : AbstractForeignKeyWriter
    {
        public SqliteForeignKeyWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            CommonDDLWriter.WriteColumnsNames(columns, builder, EscapeName);
        }

        protected override string EscapeName(string name)
        {
            return SqliteProvider.EscapeIdentifier(name);
        }
    }
}