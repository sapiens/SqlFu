using System;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders.Expressions;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServer2012Expressions : DbProviderExpressions
    {
        public SqlServer2012Expressions()
        {
           // LinkMethods(() => 1.DateDiff(TSqlDatePart.Day, DateTime.Now, DateTime.Now), DateDiff);
            
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

    public enum TSqlDatePart
    {
        Year,
        Day,
        Hour,
        Quarter,
        Month,
        DayOfYear,
        Week,
        WeekDay,
        Minute,
        Second,
        Milisecond,
        TZoffset,
        Iso_Week
    }
}