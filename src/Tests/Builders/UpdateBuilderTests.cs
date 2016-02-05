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
using Tests.Data;

namespace Tests.Builders
{
    public class UpdateBuilderTests
    {
        private IGenerateSqlFromExpressions _writer;
        private HelperOptions _options;
        private UpdateTableBuilder<Post> _sut;
        private FakeSqlExecutor _executor;
        private StringBuilder _sb;


        //public UpdateBuilderTests(ITestOutputHelper x)
        //{
        //    _writer = Setup.FakeWriter();
        //    _sb=new StringBuilder();
        //    A.CallTo(() => _writer.SqlBuffer).Returns(_sb);
        //    _executor=new FakeSqlExecutor();
        //    _options=new HelperOptions();
        //    _options.EnsureTableName(Setup.GetTableInfo<Post>());
        //    _sut =new UpdateTableBuilder<Post>(_executor,_writer,FakeEscapeIdentifier.Instance,_options);
        //}

        
        //[Fact]
        //public void set_fields()
        //{
        //    A.CallTo(() => _writer.WriteColumn(A<LambdaExpression>._)).Invokes(o => _sb.Append("SomeId"));
        //   _sut.Set(d => d.SomeId, 34).Where(d => d.Id == Guid.Empty).Execute();
        //    _executor.Result.SqlText.Should().Be("update SomePost set  where ");
        //}

    }
}