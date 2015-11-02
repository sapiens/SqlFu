using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartJoin:BaseSqlBuilderPart
    {
        private readonly string _tableName;
        private readonly LambdaExpression _value;
        private readonly ExpressionWriter _writer;

        public SqlBuilderPartJoin(string partId,string tableName,LambdaExpression value,ExpressionWriter writer):base(partId)
        {
            _tableName = tableName;
            _value = value;
        
            _writer = writer;
            
        }

        public override string ToString()
        {
            return string.Format("{0} {1} on {2}", PartId,_tableName,_writer.GetCriteriaSql(_value));            
        }
    }
}