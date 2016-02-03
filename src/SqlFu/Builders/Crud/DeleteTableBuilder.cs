using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Crud
{
    public class DeleteTableBuilder :IGenerateSql
    {
        private readonly IExpressionWriter _writer;
      

        public DeleteTableBuilder(string tableName,IExpressionWriter writer)
        {
            _writer = writer;
            _writer.Append($"delete from {tableName} ");
        }

        public void WriteCriteria(LambdaExpression expr)
        {
            _writer.Append("where ");
            _writer.WriteCriteria(expr);
        }

        public CommandConfiguration GetCommandConfiguration() => new CommandConfiguration(_writer.ToString(),_writer.Parameters.ToArray());
    }
}