#if Postgres
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


        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}
#endif