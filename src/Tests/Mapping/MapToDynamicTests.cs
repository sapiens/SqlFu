using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CavemanTools.Logging;
using CavemanTools.Testing;
using FluentAssertions;
using SqlFu.Mapping;
using SqlFu.Mapping.Internals;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Mapping
{
    public class MapToDynamicTests
    {
        public MapToDynamicTests(ITestOutputHelper x)
        {
            x.Logger();
            LogManager.OutputToTrace();
        }

        [Fact]
        public void map_to_dynamic()
        {
            var reader = Setup.FakeReader();
            var sut = new DynamicMapper();
            //var manual=new ManualMapper<dynamic>(r =>
            //{
            //    dynamic d = new ExpandoObject();
            //    d.Id = (Guid) r["Id"];
            //    d.Name = r["Name"];
            //    d.Address = r["Address"];
            //    return d;
            //});

            //var b = new BenchmarkAction(i => sut.Map(reader, ""));
            //Setup.DoBenchmark(1000, b, new BenchmarkAction(i =>
            // {
            //     manual.Map(reader);
            // }));
            dynamic poco = sut.Map(reader, "");
            AssertionExtensions.Should((object) poco.Id).Be(Guid.Empty);
            AssertionExtensions.Should((object) poco.Name).Be("bla");
            AssertionExtensions.Should((object) poco.Address).Be("");

           
        }

        [Fact]
        public void sqlfu_dynamic_can_be_cast_to_read_only_IDictionary()
        {
            var data=new SqlFuDynamic(new [] {"Id","Name"});
            Array.Copy(new object[] {1,"Test"},data.ColumnValues,2);
            dynamic dyn = data;
            var dict = (IDictionary<string, object>) dyn;
            dict["Id"].Should().Be(1);
            dict["Name"].Should().Be("Test");
            dict.IsReadOnly.Should().BeTrue();
            dict.Count.Should().Be(2);
            dict.First().Key.Should().Be("Id");
            dict.First().Value.Should().Be(1);
        }
    }
}