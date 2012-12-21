using System.Text;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlForeignKeyWriter : AbstractForeignKeyWriter
    {
        public MySqlForeignKeyWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            MySqlDDLWriter.WriteColumnsNames(columns, builder);
        }

        protected override string EscapeName(string name)
        {
            return MySqlProvider.EscapeIdentifier(name);
        }

        protected override void WriteConstraintType()
        {
            base.WriteConstraintType();
            Builder.Append(" " + Key.Name);
        }
    }
}