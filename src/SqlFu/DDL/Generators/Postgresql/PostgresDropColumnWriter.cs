using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresDropColumnWriter : AbstractDropColumnWriter
    {
        public PostgresDropColumnWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override string Escape(string name)
        {
            return PostgresProvider.EscapeIdentifier(name);
        }

        protected override void WriteEndOptions()
        {
            var c = Item.Options.Get(DropOptions.Cascade);
            if (c != null)
            {
                Builder.AppendFormat(" " + c);
            }
        }
    }
}