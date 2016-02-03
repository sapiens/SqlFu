using FluentAssertions;
using SqlFu.Mapping.Internals;
using Xunit;

namespace Tests.Mapping
{
    public class MapperFactoryTests
    {
        private MapperFactory _sut;

        public MapperFactoryTests()
        {
            _sut = Setup.MapperFactory();
        }

        [Fact]
        public void same_reader_with_different_queryId_means_different_mappers_are_created()
        {
            _sut.Map<object>(Setup.FakeReader(), "1");
            _sut.Map<object>(Setup.FakeReader(), "2");
            _sut.Mappers.Count.Should().Be(2);
        }

        [Fact]
        public void object_dynamic_mapping_uses_the_same_mapper()
        {
            _sut.Map<object>(Setup.FakeReader(), "1");
            _sut.Map<dynamic>(Setup.FakeReader(), "1");
            _sut.Mappers.Count.Should().Be(1);
        }

    }
}