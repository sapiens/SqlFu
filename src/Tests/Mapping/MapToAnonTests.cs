using System;
using FluentAssertions;
using SqlFu.Builders;
using SqlFu.Mapping;
using SqlFu.Mapping.Internals;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Mapping
{
    public class MapToAnonTests
    {
        public MapToAnonTests(ITestOutputHelper x)
        {
            x.Logger();
        
        }

        [Fact]
        public void map_to_projection()
        {
          
            var data = new {Id = 0, Title = "hi",MyEnum=OrderBy.Asc};
            var mapper = CreateMapper(data);
            var reader = Setup.FakeReader(r =>
            {
                r.Clear();
                r.Add("Id",25);
                r.Add("Tile","bla");
                r.Add("MyEnum",OrderBy.Desc);
            });

            var m = ManualMapper.For(data, r => new {Id = (int) r["Id"],Title=r["Title"].ToString(),MyEnum=(OrderBy) r["MyEnum"]});

            //Setup.DoBenchmark(10000, new[]{
            //    new BenchmarkAction(i =>
            //    {
            //        mapper.Map(reader, "");
            //    }),
            //    new BenchmarkAction(i =>
            //    {
            //        m.Map(reader);
            //    }),
            //});

            var result = mapper.Map(reader, "");    
            result.Id.Should().Be(25);
            result.Title.Should().Be("bla");
            result.MyEnum.Should().Be(OrderBy.Desc);
        }

        AnonymousTypeMapper<T> CreateMapper<T>(T data)
        {
            return new AnonymousTypeMapper<T>();
        }
    }
}