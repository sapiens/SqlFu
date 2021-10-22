using FluentAssertions;
using SqlFu;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Usage
{
	public abstract class AStoredProcsTests : IDisposable
	{
		protected DbConnection _db;

		protected AStoredProcsTests()
		{
			_db = GetConnection();
			_db.Open();
			CreateSproc();
		}

		protected abstract DbConnection GetConnection();

		protected abstract void CreateSproc();



		[Fact]
		public void execSp()
		{
			var r = _db.ExecuteSProc("spTest", new { id = 47, _pout = "" });
			r.ReturnValue.Should().Be(100);
			string pout = r.OutputValues.pout;
			Assert.Equal(pout, "bla");

		}
		[Fact]
		public async Task execSpAsync()
		{
			var r = await _db.ExecuteSProcAsync(CancellationToken.None, "spTest", new { id = 47, _pout = "" });
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
			var r = await _db.QuerySProcAsync<dynamic>(CancellationToken.None, "spTest", new { id = 49, _pout = "" });
			r.ReturnValue.Should().Be(100);
			var first = r.Result.First();
			Assert.Equal(r.OutputValues.pout, "bla");
			Assert.Equal(first.id, 1);
			Assert.Equal(first.name, "bla");
			Assert.Equal(first.input, 49);
		}

		public void Dispose()
		{
			_db.Execute("drop procedure sptest");
			_db.Dispose();
		}
	}
}