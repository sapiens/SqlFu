using System;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartCriteriaExpression<T>:BaseSqlBuilderPart
    {
        private readonly Expression<Func<T, bool>> _value;
        private readonly ExpressionWriter _writer;

        public SqlBuilderPartCriteriaExpression(string partId,Expression<Func<T,bool>> value,ExpressionWriter writer):base(partId)
        {
            _value = value;
            _writer = writer;
        }

        public override string ToString()
        {
            return _writer.GetSql(_value);            
        }
    }
}