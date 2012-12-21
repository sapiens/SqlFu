using System.Text;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteChangedColumnsManager : AbstractChangedColumnsManager
    {
        public SqliteChangedColumnsManager(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        protected override AbstractColumnChangesWriter GetWriter()
        {
            return new SqliteColumnChangesWriter(Builder);
        }
    }
}