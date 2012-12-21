using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresDropConstraintWriter : AbstractDropConstraintWriter
    {
        public PostgresDropConstraintWriter(StringBuilder builder, IDatabaseTools tools)
            : base(builder, DbEngine.PostgreSQL, tools)
        {
        }

        protected override string EscapeName(string name)
        {
            return PostgresProvider.EscapeIdentifier(name);
        }

        protected override void WriteConstraint()
        {
            Builder.AppendFormat("constraint {0}", EscapeName(Item.Name));
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