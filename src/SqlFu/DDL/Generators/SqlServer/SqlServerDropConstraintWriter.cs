using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerDropConstraintWriter : AbstractDropConstraintWriter
    {
        public SqlServerDropConstraintWriter(StringBuilder builder, IDatabaseTools tools)
            : base(builder, DbEngine.SqlServer, tools)
        {
        }

        #region Overrides of AbstractDropConstraintWriter

        protected override string EscapeName(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        protected override void WriteConstraint()
        {
            Builder.AppendFormat("constraint {0}", EscapeName(Item.Name));
        }

        #endregion
    }
}