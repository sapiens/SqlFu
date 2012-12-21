using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractIndexWriter : AbstractSchemaItemWriter
    {
        private IndexDefinition _index;

        public AbstractIndexWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected IndexDefinition Index
        {
            get { return _index; }
        }


        public virtual void Write(IndexDefinition index)
        {
            _index = index;

            if (Index.IsRedefinedFor(Engine))
            {
                Builder.AppendLine(index.GetDefinition(Engine));
                return;
            }

            index.Options.Use(Engine);

            Builder.Append("CREATE");

            WriteIndexType();

            Builder.AppendFormat(" INDEX ");
            WriteIndexName();
            Builder.Append(" ON ");
            WriteTableName();
            Builder.Append(" (");
            foreach (var idx in index.Columns)
            {
                WriteColumn(idx);
                Builder.Append(",");
            }
            Builder.RemoveLast();
            Builder.Append(")");
            WriteEndOptions();
        }

        protected virtual void WriteEndOptions()
        {
        }

        protected virtual void WriteIndexType()
        {
            if (Index.IsUnique)
            {
                Builder.Append(" UNIQUE");
            }
        }

        protected abstract void WriteIndexName();
        protected abstract void WriteTableName();
        protected abstract void WriteColumn(IndexColumn column);
    }
}