using System.Linq;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractChangedColumnsManager : AbstractSchemaItemWriter
    {
        private ModifiedColumnsCollection _columns;
        private AbstractColumnChangesWriter _writer;

        protected AbstractChangedColumnsManager(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected ModifiedColumnsCollection Columns
        {
            get { return _columns; }
        }

        public virtual void Write(ModifiedColumnsCollection columns)
        {
            _columns = columns;
            if (!columns.HasAnyChange) return;
            var changes = columns.ChangedColumns;

            foreach (var column in columns.Where(ShouldDropDefault))
            {
                Writer.WriteDropDefault(column);
            }

            foreach (var ch in changes.Where(c => c.HasChangedStructure))
            {
                Writer.WriteColumnChanges(ch);
            }

            foreach (var column in changes.Where(d => d.DefaultValue != null))
            {
                Writer.WriteSetDefault(column);
            }
        }

        protected abstract AbstractColumnChangesWriter GetWriter();

        protected AbstractColumnChangesWriter Writer
        {
            get
            {
                if (_writer == null) _writer = GetWriter();
                return _writer;
            }
        }

        protected virtual bool ShouldDropDefault(ColumnModifications col)
        {
            return col.DefaultDropped;
        }
    }
}