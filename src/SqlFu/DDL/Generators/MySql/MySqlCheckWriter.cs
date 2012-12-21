using System;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlCheckWriter : AbstractCheckWriter
    {
        public MySqlCheckWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void Write(ConstraintDefinition constraint)
        {
            //mysql ignores checks
        }

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}