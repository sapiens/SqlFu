using System.Text;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlCheckWriter:AbstractCheckWriter
    {
        public MySqlCheckWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void Write(Internals.ConstraintDefinition constraint)
        {
            //mysql ignores checks
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            throw new System.NotImplementedException();
        }
    }
}