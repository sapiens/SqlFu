using System.Text;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlChangedColumnManager : AbstractChangedColumnsManager
    {
        public MySqlChangedColumnManager(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override AbstractColumnChangesWriter GetWriter()
        {
            return new MySqlColumnChangesWriter(Builder);
        }
    }
}