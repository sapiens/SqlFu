using System.Runtime.InteropServices;
using FluentAssertions;
using SqlFu;
using SqlFu.Builders;
using SqlFu.Builders.Crud;
using SqlFu.Configuration.Internals;
using Tests.TestData;
using Tests._Fakes;
using Xunit;

namespace Tests.Builders
{
    public class SelectBuilderTests
    {
        private SimpleSqlBuilder<Post> _sut;
        private HelperOptions _opt;
        private FakeDbProvider _prov;
        private FakeWriter _writer;

        public SelectBuilderTests()
        {
            _opt=new HelperOptions(Setup.GetTableInfo<Post>());
            _prov=new FakeDbProvider();
            _writer=new FakeWriter();
            _sut =new SimpleSqlBuilder<Post>(_opt,_prov,_writer,Setup.InfoFactory());
        }

        IWhere<Post> Where => _sut as IWhere<Post>;

        [Fact]
        public void simple_query()
        {
            _writer.SetColumnsResults("grp", "sort", "*");
            _writer.SetSqlResults("c1","c2","c3","h1");
            var sql = Where.Where(d => d.SomeId == 2)
                .And(d => d.Title == "")
                .Or(d => d.Title == "")
                .GroupBy(d => d.SomeId)
                .Having(d => d.SomeId == 1)
                .OrderBy(d => d.SomeId)
                .Limit(2)
                .SelectAll()
                .GetCommandConfiguration();
            sql.SqlText.Should()
                .Be(@"select SomeId,Id,Title,Author,Email,CreatedOn from SomePost
where c1
and c2
or c3
group by grp
having h1
order by sort
 limit");
        }

    }
}