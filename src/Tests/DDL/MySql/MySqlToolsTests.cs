#if MySql
using SqlFu;
using SqlFu.Providers;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.MySql
{
    public class MySqlToolsTests:CommonDatabaseToolsTests
    {
        private Stopwatch _t = new Stopwatch();

        public MySqlToolsTests()
        {

        }

        protected override void SetupTable()
        {
            Db.ExecuteCommand(@"
create table test
( id int not null,
name varchar(20),
unique key pk_test (id)
);
create index ix_test on test(name)
");
        }

        protected override DbEngine Engine
        {
            get { return DbEngine.MySql;}
        }
        [Fact]
        public override void provider_escape_name()
        {
            Assert.Equal("`dbo.table`", MySqlProvider.EscapeIdentifier("dbo.table"));
        }

    }
}
#endif