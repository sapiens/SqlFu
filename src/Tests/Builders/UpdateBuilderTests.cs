using System;
using System.Linq.Expressions;
using System.Text;
using FakeItEasy;
using FluentAssertions;
using SqlFu.Builders.Crud;
using Tests._Fakes;
using Xunit;
using Xunit.Abstractions;
using SqlFu;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using Tests.Mocks;
using System.Linq;

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
            _options = new HelperOptions();
            _options.EnsureTableName(Setup.GetTableInfo<Post>());
            _sut = new UpdateTableBuilder<Post>(_executor, _writer, FakeEscapeIdentifier.Instance, _options);
        }


        [Fact]
        public void set_fields()
        {
           _writer.SetColumnsResults("SomeId");
            _writer.SetSqlResults("Id=@1");
            _sut.Set(d => d.SomeId, 34).Where(d => d.Id == Guid.Empty).Execute();
            _executor.Result.SqlText.Should().Be("update Post set SomeId=@0 where Id=@1");
            Parameter(0).Should().Be(34);           
        }


        [Fact]
        public void when_fields_name_are_strings()
        {
            _writer.SetSqlResults("Id=@1");
            _sut.Set("SomeId", 34).Where(d => d.Id == Guid.Empty).Execute();
            _executor.Result.SqlText.Should().Be("update Post set SomeId=@0 where Id=@1");
            Parameter(0).Should().Be(34);
        }
        object Parameter(int i) => _writer.Parameters.ToArray().Skip(i).First();
    }
}