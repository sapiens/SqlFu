using System;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Providers.Sqlite
{
    public class SqliteExpressions:DbProviderExpressions
    {
        public SqliteExpressions()
        {
            LinkMethods(() => Placeholder.Concat(), MyConcat);
        }

        private void MyConcat(MethodCallExpression method, StringBuilder sb, IGenerateSqlFromExpressions writer)
        {
            
            foreach (var arg in method.Arguments[1].CastAs<NewArrayExpression>().Expressions)
            {
                sb.Append(writer.GetColumnsSql(arg));
                sb.Append(" || ");
            }
            sb.RemoveLastIfEquals("|| ");
   
        }

        public override string Day(string column) => $"strftime('%d',{column})";

        public override string Year(string column)
            => $"strftime('%Y',{column})";

        public override string Length(string column)
            => $"length({column})";        

        public override string Substring(string column, int pos, int length)
        {
            column.MustNotBeNull();
            var idx = pos + 1;
            return $"substr({column},{idx},{length})";
        }
    }
}