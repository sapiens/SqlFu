using SqlFu;
using Xunit;

namespace Tests.Mapper
{
    public class DynamicMapperTests
    {
        public DynamicMapperTests()
        {
            
        }

        [Fact]
        public void dynamic_mapping_is_correct()
        {
            using (var db = Setup.GetDb())
            {
                var result = db.QuerySingle<dynamic>("select 1 as Id, 'bla' as Name");
                Assert.Equal(1,result.Id);
                Assert.Equal("bla",result.Name);

            }
        }

    }
}