using SqlFu.Providers;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Providers
{
    public class MySqlProviderTests
    {
        private Stopwatch _t = new Stopwatch();

        public MySqlProviderTests()
        {

        }

        [Fact]
        public void already_escaped_id_are_not_escaped()
        {
            Assert.Equal("`my table`.id", MySqlProvider.EscapeIdentifier("`my table`.id"));
            Assert.Equal("`a``b`", MySqlProvider.EscapeIdentifier("`a``b`"));
        }

        [Fact]
        public void common_escaping()
        {
            Assert.Equal("`mytable`", MySqlProvider.EscapeIdentifier("mytable"));
            Assert.Equal("`my table`", MySqlProvider.EscapeIdentifier("my table"));
        }

        [Fact]
        public void anything_with_dot_will_be_escaped_as_schema_table()
        {
            Assert.Equal("`dbo`.`mytable`", MySqlProvider.EscapeIdentifier("dbo.mytable"));
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}