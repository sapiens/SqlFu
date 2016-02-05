using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;

namespace Tests._Fakes
{
    public class FakeDbProviderExpressions:IDbProviderExpressions
    {
        public string Substring(string column, int pos, int length)
            => $"";

        public string ToUpper(string column)
        {
            throw new System.NotImplementedException();
        }

        public string ToLower(string column)
        {
            throw new System.NotImplementedException();
        }

        public string Length(string column)
        {
            throw new System.NotImplementedException();
        }

        public string Year(string column)
        {
            throw new System.NotImplementedException();
        }

        public string Day(string column)
        {
            throw new System.NotImplementedException();
        }

        public void WriteMethodCall(MethodCallExpression method, StringBuilder sb, ExpressionWriterHelper manager)
        {
            throw new System.NotImplementedException();
        }
    }
}