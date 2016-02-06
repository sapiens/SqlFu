using System;
using CavemanTools.Model.ValueObjects;
using FakeItEasy;
using FakeItEasy.ExtensionSyntax.Full;
using FluentAssertions;
using SqlFu;
using SqlFu.Builders.Crud;
using SqlFu.Configuration;
using SqlFu.Providers;
using Tests.TestData;
using Tests._Fakes;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Builders
{
    public class InsertBuilderTests
    {
        private InsertSqlBuilder _sut;
        private InsertSqlOptions _opt;
        private Post _data;
       

        public InsertBuilderTests()
        {
            _opt=new InsertSqlOptions();
            _data=new Post()
            {
                Id = Guid.NewGuid(),
                Email = Email.CreateRandomTestValue().Value,
                SomeId = 23,                
            };
          
         _sut = new InsertSqlBuilder(Setup.GetTableInfo<Post>(),_data,new FakeDbProvider(), _opt);
       
        }

        [Fact]
        public void insert_considers_primitives_and_registered_value_objects()
        {
            var cmd = _sut.GetCommandConfiguration();
            cmd.SqlText.Should().Be("insert into SomePost (Id,Title,Email,CreatedOn)\n values(@0,@1,@2,@3)");
            cmd.Args.ShouldAllBeEquivalentTo(new object[] { _data.Id, _data.Title, _data.Email, _data.CreatedOn });
        }

        [Fact]
        public void override_table_name()
        {
            _opt.TableName = "bubu";
            var cmd = _sut.GetCommandConfiguration();
            cmd.SqlText.Should().Be("insert into bubu (Id,Title,Email,CreatedOn)\n values(@0,@1,@2,@3)");
            cmd.Args.ShouldAllBeEquivalentTo(new object[] { _data.Id, _data.Title, _data.Email, _data.CreatedOn });
        }

        [Fact]
        public void override_identity_column()
        {
            _opt.IdentityColumn = "Id";
            var cmd = _sut.GetCommandConfiguration();
            cmd.SqlText.Should().Be("insert into SomePost (SomeId,Title,Email,CreatedOn)\n values(@0,@1,@2,@3)");
            cmd.Args.ShouldAllBeEquivalentTo(new object[] { _data.SomeId, _data.Title, _data.Email, _data.CreatedOn });
        }


        [Fact]
        public void specified_ignored_columns_are_ignored()
        {
            _opt.IgnoreColumns = new[] { "Title" };
            var cmd = _sut.GetCommandConfiguration();
            cmd.SqlText.Should().Be("insert into SomePost (Id,Email,CreatedOn)\n values(@0,@1,@2)");
            cmd.Args.ShouldAllBeEquivalentTo(new object[] { _data.Id, _data.Email, _data.CreatedOn });
        }
    }
}