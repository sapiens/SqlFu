using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Crud
{
    public class DeleteTableBuilder :IGenerateSql
    {
        private readonly IGenerateSqlFromExpressions _writer;
        StringBuilder _sb=new StringBuilder();

        public DeleteTableBuilder(string tableName,IGenerateSqlFromExpressions writer)
        {
            _writer = writer;
            _sb.Append($"delete from {tableName} ");
        }

        public void WriteCriteria(LambdaExpression expr)
        {
            _sb.Append("where ");
            _sb.Append(_writer.GetSql(expr));
        }

        public CommandConfiguration GetCommandConfiguration() => new CommandConfiguration(_sb.ToString(),_writer.Parameters.ToArray());
    }
}