using SqlFu;
using SqlFu.Providers;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.Postgresql
{
    public class PostgresToolsTests:CommonDatabaseToolsTests
    {
        private Stopwatch _t = new Stopwatch();

        public PostgresToolsTests()
        {

        }

        [Fact]
        public void test()
        {
            
        }

       
        protected override DbEngine Engine
        {
            get { return DbEngine.PostgreSQL;}
        }

        [Fact]
        public override void provider_escape_name()
        {
            var txt = @"""public"".""test""";
            Assert.Equal(txt,PostgresProvider.EscapeIdentifier("public.test"));
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}