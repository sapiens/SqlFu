using System.Text;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresCheckWriter : AbstractCheckWriter
    {
        public PostgresCheckWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            PostgresqlDDLWriter.WriteColumnsNames(columns, builder);
        }
    }
}