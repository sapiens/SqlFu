using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractForeignKeyWriter : AbstractConstraintWriter
    {
        public AbstractForeignKeyWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        public virtual void Write(ForeignKeyConstraint key)
        {
            base.Write(key);
            WriteParentReference();
            WriteRestrictions();
        }

        protected ForeignKeyConstraint Key
        {
            get { return Definition as ForeignKeyConstraint; }
        }

        protected virtual void WriteParentReference()
        {
            Builder.AppendFormat(" REFERENCES {0}(", EscapeName(Key.ParentTable));
            WriteColumnsNames(Key.ParentColumn, Builder);
            Builder.Append(")");
        }

        protected virtual void WriteRestrictions()
        {
            Builder.AppendFormat(" ON DELETE {0} ON UPDATE {1}", ToString(Key.OnDelete), ToString(Key.OnUpdate));
        }

        protected override void WriteConstraintType()
        {
            Builder.Append("FOREIGN KEY");
        }

        protected abstract string EscapeName(string name);


        public static string ToString(ForeignKeyRelationCascade relation)
        {
            switch (relation)
            {
                case ForeignKeyRelationCascade.Cascade:
                    return "CASCADE";
                case ForeignKeyRelationCascade.Restrict:
                    return relation.ToString();
                case ForeignKeyRelationCascade.SetDefault:
                    return "SET DEFAULT";
                case ForeignKeyRelationCascade.SetNull:
                    return "SET NULL";
                default:
                    return "NO ACTION";
            }
        }
    }
}