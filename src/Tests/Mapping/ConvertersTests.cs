using System;
using CavemanTools.Logging;
using FluentAssertions;
using SqlFu.Mapping.Internals;
using Xunit;

namespace Tests.Mapping
{
    public class ConvertersTests
    {
        private ConvertersManager _sut;

        public ConvertersTests()
        {
           _sut = new ConvertersManager();

        }

        [Fact]
        public void for_a_type_without_converter_a_default_converter_is_used()
        {
            _sut.Convert<string>((object) 3).Should().Be("3");
            _sut.Convert<string>(DBNull.Value).Should().Be(null);            
            _sut.Convert<int?>(DBNull.Value).Should().Be(null);            
        }

        [Fact]
        public void converting_from_null_to_value_throws_InvalidCastException()
        {
            _sut.Invoking(s => s.Convert<int>(null)).Should().Throw<InvalidCastException>();
        }

        [Fact]
        public void use_a_registered_converter()
        {
            _sut.AddCommonConverters();
            var guid = Guid.NewGuid();
            _sut.Convert<Guid>(guid.ToString()).Should().Be(guid);
            _sut.Convert<string>(guid).Should().Be(guid.ToString());

            var byteArr = new byte[] {1, 2, 3};
            _sut.Convert<byte[]>(byteArr).Should().BeEquivalentTo(byteArr);
        }

      
    }
}