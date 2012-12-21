using System.Text;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlDropIndexWriter : AbstractDropIndexWriter
    {
        public MySqlDropIndexWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void WriteOnTable()
        {
            Builder.AppendFormat(" on `{0}`", Item.TableName);
        }
    }
}