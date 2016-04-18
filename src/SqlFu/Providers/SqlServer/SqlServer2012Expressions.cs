namespace SqlFu.Providers.SqlServer
{
    public class SqlServer2012Expressions : DbProviderExpressions
    {
        public SqlServer2012Expressions()
        {
           // Functions.Add(typeof(MsSqlFunctions).GetMethod("FtsContains").Name, Contains);
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
}