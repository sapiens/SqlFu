using System;
using System.Data.SqlClient;
using CavemanTools.Testing;
using SqlFu.Mapping;
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
          
            var data = new {Id = 0, Title = "hi",MyEnum=SortOrder.Ascending};
            var mapper = CreateMapper(data);
            var reader = Setup.FakeReader(r =>
            {
                r.Clear();
                r["Id"] = 25;
                r["Title"] = "bla";
                r["MyEnum"] = SortOrder.Descending.ToString();
            });

            var m = ManualMapper.For(data, r =>
            {
                return new {Id = (int) r["Id"],Title=r["Title"].ToString(),MyEnum=(SortOrder) Enum.Parse(typeof(SortOrder),r["MyEnum"].ToString())};
            });

            Setup.DoBenchmark(10000, new[]{
                new BenchmarkAction(i =>
                {
                    mapper.Map(reader, "");
                }),
                new BenchmarkAction(i =>
                {
                    m.Map(reader);
                }),
            });

            var result = mapper.Map(reader, "");    
            result.Id.Should().Be(25);
            result.Title.Should().Be("bla");
            result.MyEnum.Should().Be(SortOrder.Descending);
        }

        AnonymousTypeMapper<T> CreateMapper<T>(T data)
        {
            return new AnonymousTypeMapper<T>();
        }
    }
}