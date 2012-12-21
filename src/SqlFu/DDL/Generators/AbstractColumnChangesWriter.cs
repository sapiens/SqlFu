using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractColumnChangesWriter : AbstractSchemaItemWriter
    {
        public AbstractColumnChangesWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }


        public abstract void WriteDropDefault(ColumnModifications col);

        /// <summary>
        /// type, size,collate, null
        /// </summary>
        public abstract void WriteColumnChanges(ColumnModifications col);


        public abstract void WriteSetDefault(ColumnModifications col);
    }
}