using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerDropIndexWriter : AbstractDropIndexWriter
    {
        public SqlServerDropIndexWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
        }

        #region Overrides of AbstractDropIndexWriter

        protected override void WriteOnTable()
        {
            Builder.AppendFormat(" on {0}", SqlServerProvider.EscapeIdentifier(Item.TableName));
        }

        #endregion
    }
}