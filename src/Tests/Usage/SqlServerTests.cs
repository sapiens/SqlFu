using FluentAssertions;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;
using SqlFu.Providers.SqlServer;
using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using Tests.Usage;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true, MaxParallelThreads = 1)]

namespace Tests.SqlServer
{

	[Collection("SqlServer")]
	public class SqlServerTests : ADBOperationsTests
	{
		public SqlServerTests()
		{

		}

		protected override DbConnection GetConnection()
		{
			return Setup.SqlFuConnection(new SqlServer2012Provider(SqlClientFactory.Instance.CreateConnection), Setup.SqlServerConnection);
		}

		protected override void Init()
		{

			_db.Provider().ReplaceExpressionsProvider(new MyFunctions());
			_db.AddDbObjectOrIgnore($@"
create table {_db.GetTableName<User>()} (
[Id] [int] IDENTITY(1,1) NOT NULL primary key,
FirstName nvarchar(150),
LastName nvarchar(150),
Category varchar(10) default('Page'),
CreatedOn datetime default (getdate()),
IsDeleted bit default 0,
Posts int not null default 0,
unique(FirstName)
)

");
		}

		//[Fact]
		//public void datediff()
		//{
		//	var dif = _db.QueryValue(q => q.From<User>().Select(d => d.DateDiff(TSqlDatePart.Hour, d.CreatedOn, DateTime.Now.AddHours(3))));
		//	dif.Should().Be(3);
		//}
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