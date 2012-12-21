using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlIndexWriter : AbstractIndexWriter
    {
        public MySqlIndexWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void WriteIndexName()
        {
            Builder.Append(MySqlProvider.EscapeIdentifier(Index.Name));
            var opt = Index.Options.Get(MySqlIndexOptions.Using);
            if (opt != null)
            {
                Builder.Append(" " + opt);
            }
        }

        protected override void WriteTableName()
        {
            Builder.Append(MySqlProvider.EscapeIdentifier(Index.TableName));
        }

        protected override void WriteColumn(IndexColumn column)
        {
            Builder.Append(MySqlProvider.EscapeIdentifier(column.Name));
        }
    }
}