using System;
using System.Data.SqlClient;
using System.Dynamic;
using CavemanTools.Testing;
using DomainBus.Tests;
using FluentAssertions;
using SqlFu.Mapping;
using SqlFu.Mapping.Internals;
using Xunit;
using Xunit.Abstractions;

namespace SqlFu.Tests.Mapping
{
    public class MapToDynamicTests
    {
        public MapToDynamicTests(ITestOutputHelper x)
        {
            x.Logger();
        }

        [Fact]
        public void map_to_dynamic()
        {
            var reader = Setup.FakeReader();
            var sut = new DynamicMapper();
            var m=new ManualMapper<dynamic>(r =>
            {
                dynamic d = new ExpandoObject();
                d.Id = (Guid) r["Id"];
                d.Name = r["Name"];
                d.Address = r["Address"];
                return d;
            });

            var b = new BenchmarkAction(i => sut.Map(reader, ""));
            Setup.DoBenchmark(500, b,new BenchmarkAction(i=>
            {
                m.Map(reader);
            }));
            dynamic poco = sut.Map(reader, "");
            AssertionExtensions.Should((object) poco.Id).Be(Guid.Empty);
            AssertionExtensions.Should((object) poco.Name).Be("bla");
            AssertionExtensions.Should((object) poco.Address).Be("");

           
        }

    }
}