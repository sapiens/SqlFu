using System.Text;

namespace SqlFu.DDL.Generators
{
    public abstract class AbstractSchemaItemWriter
    {
        protected readonly StringBuilder Builder;
        protected readonly DbEngine Engine;

        public AbstractSchemaItemWriter(StringBuilder builder, DbEngine engine)
        {
            Builder = builder;
            Engine = engine;
        }
    }
}