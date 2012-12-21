using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractCheckWriter : AbstractConstraintWriter
    {
        protected AbstractCheckWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        public void Write(CheckConstraint key)
        {
            base.Write(key);
        }

        protected override void WriteConstraintType()
        {
            Builder.Append("CHECK");
        }

        protected override void WriteConstraintFeature()
        {
            var c = Definition as CheckConstraint;
            Builder.Append(c.Expression);
        }
    }
}