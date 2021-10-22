using FluentAssertions;
using SqlFu.Mapping.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Mapping
{
	public class MapToDynamicTests
	{
		public MapToDynamicTests(ITestOutputHelper x)
		{
		
		}

		[Fact]
		public void map_to_dynamic()
		{
			var reader = Setup.FakeReader();
			var sut = new DynamicMapper();
			
			dynamic poco = sut.Map(reader, "");
			AssertionExtensions.Should((object)poco.Id).Be(Guid.Empty);
			AssertionExtensions.Should((object)poco.Name).Be("bla");
			AssertionExtensions.Should((object)poco.Address).Be("");


		}

		[Fact]
		public void sqlfu_dynamic_can_be_cast_to_read_only_IDictionary()
		{
			var data = new SqlFuDynamic(new[] { "Id", "Name" });
			Array.Copy(new object[] { 1, "Test" }, data.ColumnValues, 2);
			dynamic dyn = data;
			var dict = (IDictionary<string, object>)dyn;
			dict["Id"].Should().Be(1);
			dict["Name"].Should().Be("Test");
			dict.IsReadOnly.Should().BeTrue();
			dict.Count.Should().Be(2);
			dict.First().Key.Should().Be("Id");
			dict.First().Value.Should().Be(1);
		}
	}
}