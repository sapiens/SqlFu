 
using System;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using FluentAssertions;
using SqlFu;
using SqlFu.Builders.CreateTable;
using SqlFu.Configuration;
using SqlFu.Providers;
using Tests.TestData;
using Xunit;

namespace Tests.Builders
{
    //public class CreateTableBuilderTests
    //{
    //    private IDbProvider _provider;
    //    private SqlFuConfig _config;
    //    private CreateTableBuilder _sut;

    //    public CreateTableBuilderTests()
    //    {
    //        _provider = A.Fake<IDbProvider>();
    //        _config=new SqlFuConfig();
    //        _provider.CallsTo(d => d.SqlFuConfiguration).Returns(_config);
    //        _sut =new CreateTableBuilder(_provider);
    //    }

    //    [Fact]
    //    public void table_info_name_is_updated()
    //    {
    //        var data = new TableCreationData(typeof (Post));
    //        data.TableName=new TableName("test");
    //        _sut.GetSql(data);
    //        _config.TableInfoFactory.GetInfo(typeof (Post)).Table.Name.Should().Be("test");
    //    }

    //}
} 
