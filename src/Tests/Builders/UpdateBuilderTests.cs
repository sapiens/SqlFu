using FluentAssertions;
using SqlFu;
using SqlFu.Builders.Crud;
using System;
using System.Linq;
using System.Text;
using Tests._Fakes;
using Tests.TestData;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Builders
{
	public class UpdateBuilderTests
	{
		private FakeWriter _writer;
		private HelperOptions _options;
		private UpdateTableBuilder<Post> _sut;
		private FakeSqlExecutor _executor;
		private StringBuilder _sb;


		public UpdateBuilderTests(ITestOutputHelper x)
		{
			_writer = Setup.FakeWriter();
			_sb = new StringBuilder();

			_executor = new FakeSqlExecutor();
			_options = new HelperOptions(Setup.GetTableInfo<Post>());
			_sut = new UpdateTableBuilder<Post>(_executor, _writer, new FakeEscapeIdentifier(), _options);
		}


		[Fact]
		public void set_fields()
		{
			_writer.SetColumnsResults("SomeId");
			_writer.SetSqlResults("Id=@1");
			_sut.Set(d => d.SomeId, 34).Where(d => d.Id == Guid.Empty).Execute();
			_executor.Result.SqlText.Should().Be("update SomePost set SomeId=@0 where Id=@1");
			Parameter(0).Should().Be(34);
		}


		[Fact]
		public void when_fields_name_are_strings()
		{
			_writer.SetSqlResults("Id=@1");
			_sut.Set("SomeId", 34).Where(d => d.Id == Guid.Empty).Execute();
			_executor.Result.SqlText.Should().Be("update SomePost set SomeId=@0 where Id=@1");
			Parameter(0).Should().Be(34);
		}
		object Parameter(int i) => _writer.Parameters.ToArray().Skip(i).First();
	}
}