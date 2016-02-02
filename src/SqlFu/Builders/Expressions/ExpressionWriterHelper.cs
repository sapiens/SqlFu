using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.Expressions
{
    public class ExpressionWriterHelper
    {
        private readonly ITableInfoFactory _factory;
        private readonly IDbProvider _escape;

        public ExpressionWriterHelper(ITableInfoFactory factory,IDbProvider escape)
        {
            _factory = factory;
            _escape = escape;
        }

        public string GetColumnName(LambdaExpression member)
        {
            var mbody = member.Body as MemberExpression;
            if (mbody != null) return GetColumnName(mbody);
            var ubody = member.Body as UnaryExpression;
            if (ubody==null) throw new NotSupportedException("Only members and unary expressions are supported");
            return GetColumnName(ubody);
        }

        public string GetColumnName(UnaryExpression member)
        {
            var mbody = member.Operand as MemberExpression;
            if (mbody != null) return GetColumnName(mbody);
            var ubody = member.Operand as UnaryExpression;
            if (ubody != null) return GetColumnName(ubody);
            throw new NotSupportedException("Only members and unary expressions are supported");
        }


        public string GetColumnName(MemberInfo column)
        {
            return _factory.GetInfo(column.GetMemberType()).GetColumnName(column.Name,_escape);
        }
        

        public string GetColumnName(MemberExpression member)
        {
            var tableType = member.Expression.Type;
            var info = _factory.GetInfo(tableType);
            return info.GetColumnName(member, _escape);
        }

        public ExpressionWriter CreateExpressionWriter(StringBuilder sb)
        {
            return new ExpressionWriter(_escape.GetExpressionsHelper(), this, sb);
        }
    }
}