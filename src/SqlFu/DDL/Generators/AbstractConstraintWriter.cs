using System;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractConstraintWriter : AbstractSchemaItemWriter
    {
        private ConstraintDefinition _definition;

        protected AbstractConstraintWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected ConstraintDefinition Definition
        {
            get { return _definition; }
        }

        protected virtual void Write(ConstraintDefinition constraint)
        {
            _definition = constraint;
            constraint.Options.Use(Engine);
            if (constraint.IsRedefined(Engine))
            {
                Builder.Append(constraint.GetDefinition(Engine));
                return;
            }
            Builder.Append(" CONSTRAINT");
            var name = ConstraintName(constraint.Name);
            if (!name.IsNullOrEmpty())
            {
                Builder.AppendFormat(" {0} ", name);
            }
            WriteConstraintType();
            Builder.Append(" (");
            WriteConstraintFeature();
            Builder.Append(")");
        }


        protected abstract void WriteConstraintType();


        protected virtual void WriteConstraintFeature()
        {
            WriteColumnsNames(Definition.Columns, Builder);
        }

        protected virtual string ConstraintName(string name)
        {
            return name;
        }

        protected abstract void WriteColumnsNames(string columns, StringBuilder builder);
    }
}