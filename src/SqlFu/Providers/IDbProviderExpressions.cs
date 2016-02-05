using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Providers
{
    public interface IDbProviderExpressions
    {
        string Substring(string column, int pos, int length);
        string ToUpper(string column);
        string ToLower(string column);
        string Length(string column);
        string Year(string column);
        string Day(string column);
        void WriteMethodCall(MethodCallExpression method,StringBuilder sb,ExpressionWriterHelper manager);
    }
}