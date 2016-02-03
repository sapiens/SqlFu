using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Mapping
{
    public class ValueMapperTests
    {
        public ValueMapperTests(ITestOutputHelper x)
        {
            x.Logger();
        }

        [Fact]
        public void map_first_column__value()
        {
            var mapper = Setup.MapperFactory().CreateMapper<string>("1");
            var res = mapper.Map(Setup.FakeReader(r =>
              {
                  r.Clear();
                  r.Add("name", "hu");
                  r.Add("id", "hu3");
              }));
            res.Should().Be("hu");
        }

    }
}