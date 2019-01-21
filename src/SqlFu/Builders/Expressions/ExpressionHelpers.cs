using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders.Expressions
{
    public static class ExpressionHelpers
    {
        public static string GetPropertyName(this LambdaExpression node)
        {
            return GetPropertyName(node.Body);            
        }

        //public static bool IsEnumComparison(this BinaryExpression node)
        //{
        //    if (node.NodeType != ExpressionType.Equal) return false;
        //    var un = node.Left as UnaryExpression;
        //    if (un != null)
        //    {
        //        if (un.IsEnumCast()) return true;
        //    }
        //    un = node.Right as UnaryExpression;
        //    if (un != null)
        //    {
        //        if (un.IsEnumCast()) return true;
        //    }
        //    return false;
        //}

        public static bool IsEnumCast(this UnaryExpression node)
        {
            return node.NodeType == ExpressionType.Convert && node.Type == typeof(Int32) && node.Operand.Type.GetTypeInfo().IsEnum;
        }

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

        public static IEnumerable<string> GetNames<T>(this Expression<Func<T, object>>[] columns)
        {
            return columns.Select(c => GetPropertyName((LambdaExpression) c));
        }

        public static ColumnInfo[] ToColumnsInfo<T>(this TableInfo info, Expression<Func<T, object>>[] columns)
        {
            var names=columns.Select(c => c.GetPropertyName()).ToArray();
            return info.Columns.Where(c => names.Any(n => n == c.PropertyInfo.Name)).ToArray();

        }

        public static bool IsNullUnaryOrConstant(this Expression ex)
        {
            ex.MustNotBeNull();
           
            if (ex is ConstantExpression)
            {
                return ex.GetValue() == null;
            }

            var unary = ex as UnaryExpression;
            if (unary == null) return false;

            var c = unary.Operand.CastAs<ConstantExpression>();
            if (c != null)
            {
                return c.Value == null;
            }
            return false;
        }
    }
}