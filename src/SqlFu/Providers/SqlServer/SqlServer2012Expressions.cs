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
            LinkMethods(() => 1.DateDiff(TSqlDatePart.Day, DateTime.Now, DateTime.Now), DateDiff);
            // Functions.Add(typeof(MsSqlFunctions).GetMethod("FtsContains").Name, Contains);
        }

        private void DateDiff(MethodCallExpression method, StringBuilder sb, IGenerateSqlFromExpressions writer)
        {
            sb.Append("datediff(");
            sb.Append(method.Arguments[1].GetValue().ToString()).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[2])).Append(",");
            sb.Append(writer.GetColumnsSql(method.Arguments[3]));
            sb.Append(")");
        }

        //private void Contains(MethodCallExpression method, StringBuilder sb, SqlQueryManager manager)
        //{
        //    sb.Append("contains((");
        //    var writer = manager.CreateExpressionWriter(sb);
        //    method.Arguments[1].As<NewArrayExpression>().Expressions.ForEach(e =>
        //    {
        //        writer.WriteExpression(e);
        //        sb.Append(",");
        //    });
        //    sb.RemoveLastIfEquals(',');
        //    sb.Append("),");
        //    writer.WriteExpression(method.Arguments[0]);
        //    sb.Append(")");
        //}
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