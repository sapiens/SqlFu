using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractDropIndexWriter : AbstractSchemaItemWriter
    {
        private DroppedSchemaItem _item;

        public AbstractDropIndexWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected DroppedSchemaItem Item
        {
            get { return _item; }
        }

        public virtual void Write(DroppedSchemaItem item)
        {
            _item = item;
            item.Options.Use(Engine);
            Builder.AppendFormat("drop index ");
            WriteBeforeNameOptions();
            Builder.Append(item.Name);
            WriteOnTable();
            WriteEndOptions();
            Builder.Append(";");
        }

        protected virtual void WriteBeforeNameOptions()
        {
        }

        protected abstract void WriteOnTable();

        protected virtual void WriteEndOptions()
        {
        }
    }
}