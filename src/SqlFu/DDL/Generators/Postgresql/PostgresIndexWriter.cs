using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresIndexWriter : AbstractIndexWriter
    {
        public PostgresIndexWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override void WriteIndexName()
        {
            Builder.Append(Index.Name);
        }

        protected override void WriteTableName()
        {
            Builder.Append(PostgresProvider.EscapeIdentifier(Index.TableName));
        }

        protected override void WriteColumn(IndexColumn column)
        {
            Builder.Append(PostgresProvider.EscapeIdentifier(column.Name));
        }
    }
}