using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlDropColumnWriter : AbstractDropColumnWriter
    {
        public MySqlDropColumnWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override string Escape(string name)
        {
            return MySqlProvider.EscapeIdentifier(name);
        }
    }
}