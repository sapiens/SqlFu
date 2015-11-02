using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartSelectColumnsExpression:BaseSqlBuilderPart
    {
        private readonly LambdaExpression _value;
        private readonly ExpressionWriter _writer;

        public SqlBuilderPartSelectColumnsExpression(string partId,LambdaExpression value,ExpressionWriter writer):base(partId)
        {
            _value = value;
            _writer = writer;
        }

        public override string ToString()
        {
            return _writer.GetSelectColumnsSql(_value);            
        }
    }
}