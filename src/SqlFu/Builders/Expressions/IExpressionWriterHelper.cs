using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SqlFu.Builders.Expressions
{
    public interface IExpressionWriterHelper
    {
        string GetColumnName(LambdaExpression member);
        string GetColumnName(UnaryExpression member);
        string GetColumnName(MemberInfo column);
        string GetColumnName(MemberExpression member);
        IExpressionWriter CreateExpressionWriter(StringBuilder sb=null);
    }
}