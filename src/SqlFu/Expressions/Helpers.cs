using System;
using System.Linq.Expressions;

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

        internal static bool IsNullUnaryOrConstant(this Expression ex)
        {
            ex.MustNotBeNull();
            //if (ex == null) return false;

            if (ex is ConstantExpression)
            {
                return ex.GetValue() == null;
            }

            var unary = ex as UnaryExpression;
            if (unary == null) return false;

            var c = unary.Operand.As<ConstantExpression>();
            if (c != null)
            {
                return c.Value == null;
            }
            return false;
        }
    }
}