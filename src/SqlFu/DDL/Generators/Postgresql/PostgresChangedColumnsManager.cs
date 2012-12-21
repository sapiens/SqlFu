using System.Text;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresChangedColumnsManager : AbstractChangedColumnsManager
    {
        public PostgresChangedColumnsManager(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        protected override AbstractColumnChangesWriter GetWriter()
        {
            return new PostgresColumnChangesWriter(Builder);
        }
    }
}