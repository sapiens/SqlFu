using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using SqlFu;
using Xunit;
using Xunit.Abstractions;

namespace Tests.SqlServer
{
    public class StoredProcsTests : IDisposable
    {
        private DbConnection _db;

        public StoredProcsTests(ITestOutputHelper h)
        {
       
            _db = Setup.GetConnection();
            h.WriteLine("xxxxxxxxxxx");
            Init();
        }



        void Init()
        {
            _db.AddDbObjectOrIgnore(@"CREATE PROCEDURE spTest
	@id int,
	@pout varchar(50) out
AS
BEGIN
		SET NOCOUNT ON;
   set @pout='bla';
	SELECT 1 as id,'bla' as name, @id as [input];
	return 100;
END");
          

        }

        [Fact]
        public void execSp()
        {
            var r = _db.ExecuteSProc(s =>
              {
                  s.ProcName = "spTest";
                  s.Arguments = new { id = 47, _pout = "" };
              });
            r.ReturnValue.Should().Be(100);
            string pout = r.OutputValues.pout;
            Assert.Equal(pout, "bla");

        }
        [Fact]
        public async Task execSpAsync()
        {
            var r = await _db.ExecuteSProcAsync(s =>
              {
                  s.ProcName = "spTest";
                  s.Arguments = new { id = 47, _pout = "" };
              }, CancellationToken.None);
            r.ReturnValue.Should().Be(100);
            string pout = r.OutputValues.pout;
            Assert.Equal(pout, "bla");

        }

        [Fact]
        public void querySp()
        {
            var r = _db.QuerySProc<dynamic>("spTest", new { id = 46, _pout = "" });
            r.ReturnValue.Should().Be(100);
            var first = r.Result.First();
            Assert.Equal(r.OutputValues.pout, "bla");
            Assert.Equal(first.id, 1);
            Assert.Equal(first.name, "bla");
            Assert.Equal(first.input, 46);
        }
        [Fact]
        public async Task querySpAsync()
        {
            var r = await _db.QuerySProcAsync<dynamic>("spTest", CancellationToken.None, new { id = 49, _pout = "" });
            r.ReturnValue.Should().Be(100);
            var first = r.Result.First();
            Assert.Equal(r.OutputValues.pout, "bla");
            Assert.Equal(first.id, 1);
            Assert.Equal(first.name, "bla");
            Assert.Equal(first.input, 49);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}