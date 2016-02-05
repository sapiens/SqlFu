using System.Linq;
using CavemanTools.Logging;
using FakeItEasy;
using FluentAssertions;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping;
using Tests.Mocks;
using Xunit;

namespace Tests.Configuration
{
    public class TableInfoTests
    {
        private TableInfo _sut;
        private IManageConverters _manageConverters;

        public TableInfoTests()
        {
            LogManager.OutputToConsole();
            _manageConverters = A.Fake<IManageConverters>();
            _sut = new TableInfo(typeof(MapperPost), _manageConverters);
        }

        [Fact]
        public void all_properties_are_considered_columns()
        {
            _sut.Columns.Count().Should().Be(13);

        }

        [Fact]
        public void table_name_is_type_name()
        {
            _sut.Table.Name.Should().Be("MapperPost");
        }

        [Fact]
        public void name_from_attribute()
        {
            var sut=new TableInfo(typeof(Post),_manageConverters);
            sut.HandleAttributeOverride();
            sut.Table.Name.Should().Be("SomePost");
            sut.IdentityColumn.Should().Be("SomeId");
        }

        [Fact]
        public void name_from_convention()
        {
            var factory = Setup.InfoFactory();
            factory.AddNamingConvention(t=>t==typeof(MapperPost),t=> new TableName("Post1"));

            var info = factory.GetInfo(typeof (MapperPost));
            info.Table.Name.Should().Be("Post1");
        }

        [Fact]
        public void attribute_override_convention()
        {
            var factory = Setup.InfoFactory();
            factory.AddNamingConvention(t => t == typeof(Post), t => new TableName("Post1"));

            var info = factory.GetInfo(typeof(Post));
            info.Table.Name.Should().Be("SomePost");
        }
    }
}