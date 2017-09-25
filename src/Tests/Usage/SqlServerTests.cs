using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;
using SqlFu.Providers.SqlServer;

namespace Tests.SqlServer
{
    public class SqlServerTests : ADBOperationsTests
    {
        public static string ConnectionString =>
            Setup.IsAppVeyor
                ? @"Server=(local)\SQL2016;Database=tempdb;User ID=sa;Password=Password12!"
                : @"Data Source=.\SQLExpress;Initial Catalog=tempdb;Integrated Security=True;MultipleActiveResultSets=True";

        public SqlServerTests()
        {

        }

        protected override DbConnection GetConnection()
        {
            return Setup.SqlFuConnection(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection),
                ConnectionString);
        }

        protected override void Init()
        {
   
            _db.Provider().ReplaceExpressionsProvider(new MyFunctions());
            _db.AddDbObjectOrIgnore($@"
create table {_db.GetTableName<User>()} (
Id int identity(1,1) not null primary key,
FirstName nvarchar(150),
LastName nvarchar(150),
Category varchar(10) default('Page'),
CreatedOn datetime ,
IsDeleted bit default 0,
Posts int not null
)
");
        }
    }


    public static class MyExtensions
    {
        public static int DateDiff<T>(this T table, TSqlDatePart part, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }

    public class MyFunctions : DbProviderExpressions
    {
        public MyFunctions()
        {
            LinkMethods(() => 1.DateDiff(TSqlDatePart.Day, DateTime.Now, DateTime.Now), DateDiff);
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
        Hour
        //etc
    }

}