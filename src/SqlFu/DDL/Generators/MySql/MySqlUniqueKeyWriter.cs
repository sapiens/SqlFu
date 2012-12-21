using System.Text;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlUniqueKeyWriter : AbstractUniqueKeyWriter
    {
        public MySqlUniqueKeyWriter(StringBuilder builder)
            : base(builder, DbEngine.MySql)
        {
        }


        protected override void WriteConstraintType()
        {
            base.WriteConstraintType();
            if (!Key.IsPrimary)
            {
                Builder.Append(" KEY " + Key.Name);
            }
            var opt = Key.Options.Get(MySqlIndexOptions.Using);
            if (opt != null)
            {
                Builder.Append(" " + opt);
            }
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            MySqlDDLWriter.WriteColumnsNames(columns, builder);
        }
    }
}