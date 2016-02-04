using System;
using System.Linq.Expressions;
using System.Text;

namespace SqlFu.Builders.Expressions
{
    public interface IExpressionWriter
    {
        StringBuilder SqlBuffer { get; }
        ParametersManager Parameters { get; }
        IExpressionWriterHelper Helper { get; }
      
        void WriteColumn(LambdaExpression property);
        string GetColumnsSql(params LambdaExpression[] columns);
        string GetSql<T>(Expression<Func<T, object>> criteria);
        string GetSql<T>(Expression<Func<T, bool>> criteria);
        string GetCriteriaSql(LambdaExpression criteria);
        string GetExpressionSql(LambdaExpression expression);
        void WriteCriteria(LambdaExpression criteria);
        void WriteExpression(LambdaExpression expression);
        void WriteExpression(Expression expression);

        void Append(string sql,bool withNewLine=false);
        void AppendLine();

    }
}