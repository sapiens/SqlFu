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
        void Write<T>(Expression<Func<T, bool>> criteria);
        void WriteColumn(LambdaExpression property);
        void WriteColumn(MemberExpression property);
        string GetSelectColumnsSql(params LambdaExpression[] columns);
        string GetSql<T>(Expression<Func<T, object>> criteria);
        string GetSql<T>(Expression<Func<T, bool>> criteria);
        string GetCriteriaSql(LambdaExpression criteria);
        string GetExpressionSql(LambdaExpression expression);
        void WriteCriteria(LambdaExpression criteria);
        void WriteExpression(LambdaExpression expression);
        void WriteExpression(Expression expression);
        void Write<T>(Expression<Func<T, object>> expression);

        void Append(string sql);
        // Expression Visit(Expression node);      

    }
}