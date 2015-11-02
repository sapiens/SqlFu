using System.Linq.Expressions;
using SqlFu.Builders.Expressions;

namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartOrderBy : BaseSqlBuilderPart
    {
        private readonly LambdaExpression _column;
        private readonly OrderBy _sort;
        private readonly ExpressionWriter _writer;

        public SqlBuilderPartOrderBy(LambdaExpression column,OrderBy sort,ExpressionWriter writer) : base(ASqlBuilder.OrderBy_)
        {
            _column = column;
            _sort = sort;
            _writer = writer;
        }

        public override string ToString()
        {
            //_writer.Manager.SimpleColumnNames = true;
            //var re = _writer.GetSelectColumnsSql(_value);
            //_writer.Manager.SimpleColumnNames = false;
            //return re;
            return string.Format("{0} {1}",_writer.GetExpressionSql(_column,true),_sort);
        }
    }
}