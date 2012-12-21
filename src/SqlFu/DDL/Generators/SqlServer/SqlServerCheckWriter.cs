using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerCheckWriter : AbstractCheckWriter
    {
        public SqlServerCheckWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        public SqlServerCheckWriter(StringBuilder builder) : this(builder, DbEngine.SqlServer)
        {
        }

        protected override string ConstraintName(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            SqlServerDDLWriter.WriteColumnsNames(columns, Builder);
        }
    }
}