using System.Data.Common;
using CavemanTools;
using CavemanTools.Model;
using FluentAssertions;
using NSubstitute;
using SqlFu.Builders;
using SqlFu.Providers.SqlServer;
using Xunit;

namespace Tests.Providers
{
    public class SqlServer2012Tests
    {
        private SqlServer2012Provider _sut;

        public SqlServer2012Tests()
        {
            _sut=new SqlServer2012Provider(Empty.Func<DbConnection>());
        }

        [Fact]
        public void escape_is_done_properly()
        {
            _sut.EscapeIdentifier("haha").Should().Be("[haha]");
        }

        [Fact]
        public void exception_when_object_exists()
        {
            var ex = Substitute.For<DbException>();
            ex.Message.Returns("There is already an object named 'IsSomething ' in the database.");
            _sut.ObjectExists(ex).Should().BeTrue();
            _sut.ObjectExists(ex, "IsSomething").Should().BeTrue();
            _sut.ObjectExists(ex, "Nope").Should().BeFalse();

        }

        [Fact]
        public void format_pagination()
        {
            var pm=new ParametersManager();
            var page = new Pagination();
            _sut.FormatQueryPagination("", page, pm)
            .Should().Be(" order by 1 OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY");
            pm.ToArray().ShouldAllBeEquivalentTo(new[] {page.Skip,page.PageSize});
        }

        [Fact]
        public void format_pagination_keeps_existing_order_by()
        {
            var pm=new ParametersManager();
            var page = new Pagination();
            _sut.FormatQueryPagination("order by Id", page, pm)
            .Should().Be("order by Id OFFSET @0 ROWS FETCH NEXT @1 ROWS ONLY");
            pm.ToArray().ShouldAllBeEquivalentTo(new[] {page.Skip,page.PageSize});
        }
    }
}