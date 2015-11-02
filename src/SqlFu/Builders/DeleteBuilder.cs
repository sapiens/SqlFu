using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders
{
    public class DeleteBuilder<T>:IGenerateSql
    {
        private readonly ExpressionWriter _writer;
        private StringBuilder _sb;

        public DeleteBuilder(DbConnection db)
        {
            _sb=new StringBuilder();
            _writer = db.CreateWriterHelper().CreateExpressionWriter(_sb);

            var name = db.GetPocoInfo<T>().EscapeName(db.GetProvider());
            _sb.Append($"delete from {name} ");
        }

        public void WriteCriteria(LambdaExpression expr)
        {
            _sb.Append("where ");
            _writer.WriteCriteria(expr);
        }

        public CommandConfiguration GetCommandConfiguration()
        {
            return new CommandConfiguration(_sb.ToString(),_writer.Parameters.ToArray());
        }
    }
}