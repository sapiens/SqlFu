using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SqlFu.Tests.Providers;
using SqlFu.Tests._Fakes;
using Xunit;

namespace SqlFu.Tests.Executors
{
    public class QueryExtensionsTests:IDisposable
    {
        private DbConnection _db;

        public QueryExtensionsTests()
        {
            
            

            _db = Setup.GetConnection();

         //   _db.CreateTableFrom<SomePost>();
        }

        [Fact]
        public async Task paged_query()
        {
            using (var t = _db.BeginTransaction())
            {
                //var res = await _db.From<SomePost>().Select().Limit(10, 0).GetPagedAsync(CancellationToken.None);
                //res.Count.Should().Be(0);

                //await _db.InsertAsync<SomePost>(new SomePost(){Content = "bla",State = SomeEnum.Last,Title="title"});
                //var func = SqlServer2012Provider.Instance.Functions;
                //var res1 = await _db.From<SomePost>().Select(d=>new{d.Id,Total=func.Sum(d.Id)}).GroupBy(d=>d.Id).Limit(10,0).GetPagedAsync(CancellationToken.None);
                //res1.Count.Should().Be(1);
                ////res1.Items.First().Title.Should().Be("title");
            }
            
        }

        public void Dispose()
        {
            _db.DropTable<SomePost>();
            _db.Dispose();
        }
    }
}