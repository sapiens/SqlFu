//using Xunit;
//using Xunit.Abstractions;

//namespace Tests.Mapping
//{
//    public class ValueMapperTests
//    {
//        public ValueMapperTests(ITestOutputHelper x)
//        {
//            x.Logger();
//        }

//        [Fact]
//        public void map_one_value()
//        {
//            var mapper=Setup.MapperFactory().CreateMapper<string>("1");
//            var res=mapper.Map(Setup.FakeReader(r =>
//            {
//                r.Clear();
//                r["name"] = "hu";
//                r["id"] = "hu3";
//            }));
//            res.Should().Be("hu");
//        }

//    }
//}