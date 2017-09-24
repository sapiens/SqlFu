using System;
using System.Linq.Expressions;
using System.Text;
using FluentAssertions;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;
using Tests.TestData;
using Xunit;

namespace Tests.Providers
{

    public enum TSqlDatePart
    {
        Year,
        Day,
        Hour
        //etc
    }
    public static class MyExtensions
    {
        public static int DateDiff<T>(this T table, TSqlDatePart part, DateTime startDate,DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }

    public class MyFunctions : DbProviderExpressions
    {
        public MyFunctions()
        {
            LinkMethods(()=>1.DateDiff(TSqlDatePart.Day, DateTime.Now, DateTime.Now),DateDiff);    
        }

        private void DateDiff(MethodCallExpression method, StringBuilder sb, IGenerateSqlFromExpressions writer)
        {
            sb.Append("datediff(");
            sb.Append(method.Arguments[1].GetValue().ToString()).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[2])).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[3]));
            sb.Append(")");
        }

    }


    public class AddingCustomProviderExpressions
    {
        private ExpressionSqlGenerator _sut;

        public AddingCustomProviderExpressions()
        {
            _sut = Setup.CreateExpressionSqlGenerator(new MyFunctions());
        }

        [Fact]
        public void value_objects_are_flattened_on_setup_param()
        {
            Expression<Func<MapperPost, object>> l = x => x.DateDiff(TSqlDatePart.Day, x.CreatedOn, DateTime.Now);
            var rez = _sut.GetSql(l);
            rez.Should().Be("datediff(Day,CreatedOn,@0)");
        }

    }
}