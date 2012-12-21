using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerForeignKeyWriter : AbstractForeignKeyWriter
    {
        public SqlServerForeignKeyWriter(StringBuilder builder)
            : base(builder, DbEngine.SqlServer)
        {
        }

        public SqlServerForeignKeyWriter(StringBuilder builder, DbEngine engine)
            : base(builder, engine)
        {
        }


        protected override string ConstraintName(string name)
        {
            return EscapeName(name);
        }

        protected override string EscapeName(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            SqlServerDDLWriter.WriteColumnsNames(columns, Builder);
        }
    }
}