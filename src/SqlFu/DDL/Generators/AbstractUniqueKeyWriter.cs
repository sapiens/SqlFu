using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractUniqueKeyWriter : AbstractConstraintWriter
    {
        protected AbstractUniqueKeyWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        public virtual void Write(UniqueKeyConstraint constraint)
        {
            base.Write(constraint);
        }

        protected override void WriteConstraintType()
        {
            var key = Definition as UniqueKeyConstraint;
            if (key.IsPrimary)
            {
                Builder.Append("PRIMARY KEY");
            }

            else
            {
                Builder.Append("UNIQUE");
            }
        }

        protected UniqueKeyConstraint Key
        {
            get { return Definition as UniqueKeyConstraint; }
        }
    }
}