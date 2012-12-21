using System.Text;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerUniqueKeyWriter : AbstractUniqueKeyWriter
    {
        public SqlServerUniqueKeyWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
        }

        protected SqlServerUniqueKeyWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected override string ConstraintName(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        protected override void WriteConstraintType()
        {
            base.WriteConstraintType();
            Builder.Append(GetOptionsBeforeColumns(Key.Options));
        }

        protected virtual string GetOptionsBeforeColumns(DbEngineOptions options)
        {
            var option = options.Get(SqlServerOptions.Clustered);
            if (option != null)
            {
                return " " + option;
            }

            option = options.Get(SqlServerOptions.NonClustered);
            if (option != null)
            {
                return " " + option;
            }
            return "";
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            SqlServerDDLWriter.WriteColumnsNames(columns, Builder);
        }
    }
}