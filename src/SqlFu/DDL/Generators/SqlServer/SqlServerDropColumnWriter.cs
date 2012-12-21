using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerDropColumnWriter : AbstractDropColumnWriter
    {
        public SqlServerDropColumnWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
        }

        #region Overrides of AbstractDropColumnWriter

        protected override string Escape(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        #endregion
    }
}