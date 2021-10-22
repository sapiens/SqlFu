using CavemanTools;
using CavemanTools.Model;
using FluentAssertions;
using NSubstitute;
using SqlFu.Builders;
using SqlFu.Providers.SqlServer;
using System;
using System.Data.Common;
using System.Linq.Expressions;
using Tests.TestData;
using Xunit;

namespace Tests.Providers
{
	public class SqlServer2012Tests
	{
		private SqlServer2012Provider _sut;

		public SqlServer2012Tests()
		{
			_sut = new SqlServer2012Provider(Empty.Func<DbConnection>());
		}

		[Fact]
		public void escape_is_done_properly()
		{
			_sut.EscapeIdentifier("haha").Should().Be("[haha]");
		}

		[Fact]
		public void format_pagination()
		{
			var pm = new ParametersManager();
			var page = new Pagination();
			_sut.FormatQueryPagination("", page, pm)
			.Should().Be(" order by 1 OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY");
			pm.ToArray().Should().BeEquivalentTo(new[] { page.Skip, page.PageSize });
		}

		//[Fact]
		//public void date_diff()
		//{
		//	var sut = Setup.CreateExpressionSqlGenerator(new SqlServer2012Expressions());
		//	Expression<Func<MapperPost, object>> l = x => x.DateDiff(TSqlDatePart.Day, x.CreatedOn, DateTime.Now);
		//	var rez = sut.GetSql(l);
		//	rez.Should().Be("datediff(Day,CreatedOn,@0)");
		//}

		[Fact]
		public void format_pagination_keeps_existing_order_by()
		{
			var pm = new ParametersManager();
			var page = new Pagination();
			_sut.FormatQueryPagination("order by Id", page, pm)
			.Should().Be("order by Id OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY");
			pm.ToArray().Should().BeEquivalentTo(new[] { page.Skip, page.PageSize });
		}
	}
}