using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteDropColumnWriter : AbstractDropColumnWriter
    {
        public SqliteDropColumnWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        protected override string Escape(string name)
        {
            return SqliteProvider.EscapeIdentifier(name);
        }
    }
}