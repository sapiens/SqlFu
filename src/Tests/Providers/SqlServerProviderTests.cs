using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Providers
{
    public class SqlServerProviderTests
    {
        private Stopwatch _t = new Stopwatch();

        public SqlServerProviderTests()
        {

        }

        [Fact]
        public void already_escaped_id_are_not_escaped()
        {
            Assert.Equal("dbo.[my.table]", SqlServerProvider.EscapeIdentifier("dbo.[my.table]"));
            Assert.Equal("dbo.\"my.table\"", SqlServerProvider.EscapeIdentifier("dbo.\"my.table\""));
        }

        [Fact]
        public void common_escaping()
        {
            Assert.Equal("[mytable]",SqlServerProvider.EscapeIdentifier("mytable"));
            Assert.Equal("[my table]",SqlServerProvider.EscapeIdentifier("my table"));
        }

        [Fact]
        public void anything_with_dot_will_be_escaped_as_schema_table()
        {
            Assert.Equal("[dbo].[mytable]", SqlServerProvider.EscapeIdentifier("dbo.mytable"));
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}