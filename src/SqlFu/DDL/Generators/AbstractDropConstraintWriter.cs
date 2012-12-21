using System;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractDropConstraintWriter : AbstractSchemaItemWriter
    {
        private readonly IDatabaseTools _tools;
        private DroppedSchemaItem _item;

        public AbstractDropConstraintWriter(StringBuilder builder, DbEngine engine, IDatabaseTools tools)
            : base(builder, engine)
        {
            _tools = tools;
        }

        protected DroppedSchemaItem Item
        {
            get { return _item; }
        }

        public virtual void Write(DroppedSchemaItem item)
        {
            _item = item;
            Item.Options.Use(Engine);
            if (Item.Name.IsNullOrEmpty())
            {
                Item.Name = _tools.GetPrimaryKeyName(Item.TableName);
            }
            if (!Item.Name.IsNullOrEmpty())
            {
                Builder.AppendFormat("alter table {0} drop ", EscapeName(item.TableName));
                WriteConstraint();
                WriteEndOptions();
                Builder.Append(";");
            }
        }

        protected abstract string EscapeName(string name);
        protected abstract void WriteConstraint();

        protected virtual void WriteEndOptions()
        {
        }
    }
}