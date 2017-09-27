using System.Linq;
using CavemanTools.Logging;
using FluentAssertions;
using NSubstitute;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping;
using Tests.TestData;
using Xunit;

namespace Tests.Configuration
{
    public class TableInfoTests
    {
        private TableInfo _sut;
        private IManageConverters _manageConverters;

        public TableInfoTests()
        {
            _manageConverters = Substitute.For<IManageConverters>();
            _sut = new TableInfo(typeof(MapperPost), _manageConverters);
        }

        [Fact]
        public void all_properties_are_considered_columns()
        {
            _sut.Columns.Count().Should().Be(14);

        }

        [Fact]
        public void table_name_is_type_name()
        {
            _sut.TableName.Name.Should().Be("MapperPost");
        }

      
        [Fact]
        public void name_from_convention()
        {
            var factory = Setup.InfoFactory();
            factory.AddNamingConvention(t=>t==typeof(MapperPost),t=> new TableName("Post1"));

            var info = factory.GetInfo(typeof (MapperPost));
            info.TableName.Name.Should().Be("Post1");
        }

     
    }
}