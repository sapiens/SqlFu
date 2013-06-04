#if MySQl
using SqlFu;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.MySql
{

    public class MySqlAlterTableTests:CommonAlterTableTests
    {
        private Stopwatch _t = new Stopwatch();

        public MySqlAlterTableTests()
        {

        }


        protected override DbEngine Engine
        {
            get { return DbEngine.MySql;}
        }

        [Fact(Skip = "MySql ignores checks")]
        public override void drop_check()
        {
            base.drop_check();
        }
        protected override void do_drop_unique()
        {
            Table.Indexes.Drop("uk_others").WithoutOptions().ExecuteDDL();
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}
#endif