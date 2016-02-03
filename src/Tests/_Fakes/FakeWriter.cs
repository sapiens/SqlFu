using System;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders;
using SqlFu.Builders.Expressions;

namespace Tests._Fakes
{
    public class FakeWriter:IExpressionWriter
    {
        public FakeWriter()
        {
            
        }
        public StringBuilder SqlBuffer { get; }=new StringBuilder();
        public ParametersManager Parameters { get; }= new ParametersManager();
        public IExpressionWriterHelper Helper { get; }
        public void Write<T>(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public void WriteColumn(LambdaExpression property)
        {
            throw new NotImplementedException();
        }

        public void WriteColumn(MemberExpression property)
        {
            throw new NotImplementedException();
        }

        public string GetSelectColumnsSql(params LambdaExpression[] columns)
        {
            throw new NotImplementedException();
        }

        public string GetSql<T>(Expression<Func<T, object>> criteria)
        {
            throw new NotImplementedException();
        }

        public string GetSql<T>(Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public string GetCriteriaSql(LambdaExpression criteria)
        {
            throw new NotImplementedException();
        }

        public string GetExpressionSql(LambdaExpression expression)
        {
            throw new NotImplementedException();
        }

        public void WriteCriteria(LambdaExpression criteria)
        {
            throw new NotImplementedException();
        }

        public void WriteExpression(LambdaExpression expression)
        {
            throw new NotImplementedException();
        }

        public void WriteExpression(Expression expression)
        {
            throw new NotImplementedException();
        }

        public void Write<T>(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public void Append(string sql, bool withNewLine = false)
        {
            throw new NotImplementedException();
        }

        public void AppendLine()
        {
            throw new NotImplementedException();
        }
    }
}