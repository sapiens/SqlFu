using SqlFu;
using Xunit;
using System;
using System.Diagnostics;


namespace Tests.Mapper
{
    public class SimpleMapper
    {
        private Stopwatch _t = new Stopwatch();

        public SimpleMapper()
        {

        }

        [Fact]
        public void map_to_poco_with_private_constructor()
        {
            using (var db = Setup.GetDb())
            {
                Assert.DoesNotThrow(()=> db.QuerySingle<OtherPost>("select 1"));
            }
        }


        [Fact]
        public void map_to_property_with_private_setter()
        {
            using (var db = Setup.GetDb())
            {
                var p = db.QuerySingle<OtherPost>("select 'hy' as Name");
                Assert.Equal(p.Name, "hy");
            }
        }

       
        protected void Write(object format, params object[] param)
        {
            Console.WriteLine(format.ToString(), param);
        }
    }
}