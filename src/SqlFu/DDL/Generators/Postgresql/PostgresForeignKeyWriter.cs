using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresForeignKeyWriter : AbstractForeignKeyWriter
    {
        public PostgresForeignKeyWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            PostgresqlDDLWriter.WriteColumnsNames(columns, builder);
        }

        protected override string EscapeName(string name)
        {
            return PostgresProvider.EscapeIdentifier(name);
        }
    }
}