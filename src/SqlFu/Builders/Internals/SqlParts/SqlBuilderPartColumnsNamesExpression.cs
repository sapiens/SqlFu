using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartColumnsNamesExpression:BaseSqlBuilderPart
    {
        private readonly LambdaExpression[] _value;
        private readonly ExpressionWriter _writer;

        public SqlBuilderPartColumnsNamesExpression(string partId, ExpressionWriter writer,params LambdaExpression[] values):base(partId)
        {
            _value = values;
            _writer = writer;
        }

        public override string ToString()
        {
            _writer.Manager.SimpleColumnNames = true;
            var re= _writer.GetSelectColumnsSql(_value);
            _writer.Manager.SimpleColumnNames = false;
            return re;
        }
    }
}