using System.Text;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresDropIndexWriter : AbstractDropIndexWriter
    {
        public PostgresDropIndexWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override void WriteOnTable()
        {
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