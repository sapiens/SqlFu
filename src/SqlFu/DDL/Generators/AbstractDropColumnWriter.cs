using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractDropColumnWriter : AbstractSchemaItemWriter
    {
        private ColumnModifications _item;

        public AbstractDropColumnWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected ColumnModifications Item
        {
            get { return _item; }
        }

        public virtual void Write(ColumnModifications item)
        {
            _item = item;
            _item.Options.Use(Engine);
            Builder.AppendFormat("alter table {0} drop column {1}", Escape(item.TableName), Escape(item.Name));
            WriteEndOptions();
            Builder.AppendLine(";");
        }

        protected abstract string Escape(string name);

        protected virtual void WriteEndOptions()
        {
        }
    }
}