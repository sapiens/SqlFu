using FluentAssertions;
using SqlFu.Builders.Crud;
using System;
using System.Linq;
using System.Linq.Expressions;
using Tests._Fakes;
using Tests.TestData;
using Xunit;

namespace Tests.Builders
{
	public class DeleteTableBuilderTests
	{
		private FakeWriter _writer;
		private DeleteTableBuilder _sut;

		public DeleteTableBuilderTests()
		{
			_writer = Setup.FakeWriter();
			_sut = new DeleteTableBuilder("Post", _writer);
		}

		[Fact]
		public void delete_without_criteria()
		{
			var cmd = _sut.GetCommandConfiguration();
			cmd.SqlText.Should().Be("delete from Post");
		}

		[Fact]
		public void delete_with_criteria()
		{
			Expression<Func<Post, object>> l = d => d.SomeId == 23;
			_writer.SetSqlResults("(SomeId = @0)");
			_writer.Parameters.AddValues(23);
			_sut.WriteCriteria(l);
			var cmd = _sut.GetCommandConfiguration();
			cmd.SqlText.Should().Be("delete from Post where (SomeId = @0)");
			cmd.Args.First().Should().Be(23);
		}
	}
}