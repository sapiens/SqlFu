using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;
using SqlFu.Builders.Internals;
using SqlFu.Builders.Internals.SqlParts;

namespace SqlFu.Builders
{
    public class SqlParts:List<ISqlBuilderPart>
    {
        private readonly ExpressionWriter _writer;

        public SqlParts(ExpressionWriter writer)
        {
            _writer = writer;
        }

        public void Add(string partId, string value)
        {
            partId.MustNotBeEmpty();
            Add(new SqlBuilderPartValue(partId,value));
        }

        public void Add<T>(string partId,Expression<Func<T, bool>> expression)
        {
            Add(new SqlBuilderPartCriteriaExpression<T>(partId,expression,_writer));
        }

        public ISqlBuilderPart[] GetParts(Func<string,bool> selector)
        {
            return this.Where(d => selector(d.PartId)).ToArray();
        }

        
    }
}