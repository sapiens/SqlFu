using SqlFu;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.SqlServer
{
    public class SqlServerToolsTests:CommonDatabaseToolsTests
    {
        private Stopwatch _t = new Stopwatch();

        public SqlServerToolsTests()
        {

        }


        protected override DbEngine Engine
        {
            get { return DbEngine.SqlServer;}
        }
       

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}