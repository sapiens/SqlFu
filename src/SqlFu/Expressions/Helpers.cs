using System.Linq.Expressions;
using System;

namespace SqlFu.Expressions
{
    public static class Helpers
    {
         public static string GetPropertyName(this Expression node)
         {
             var member = node as MemberExpression;
             if (member == null)
             {
                 var unary = node as UnaryExpression;
                 if (unary == null)
                 {
                     throw new ArgumentException("Expression must be MemberExpression or UnaryExpression");
                 }
                 member = unary.Operand as MemberExpression;
             }
             if (member == null)
             {
                 throw new ArgumentException("Expression isn't a member access");
             }

             return member.Member.Name;
         }

        internal static ExpressionSqlBuilder<T> WriteDelete<T>(this ExpressionSqlBuilder<T> builder)
        {
            builder.Append("delete from ").WriteTableName();
            return builder;
        }
    }
}