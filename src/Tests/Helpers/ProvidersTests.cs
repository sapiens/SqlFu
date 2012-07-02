using SqlFu;
using SqlFu.Providers;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Helpers
{
    public class ProvidersTests
    {
        private Stopwatch _t = new Stopwatch();

        public ProvidersTests()
        {

        }

        [Fact]
        public void mysql_test_pagination_strings()
        {
            var m = new MySqlProvider(Config.SqlServerProviderName);
            string cnt;
            string sel;
            m.MakePaged("select * from test",out sel,out cnt);
            Assert.Equal("select count(*) from test",cnt);
            Assert.Equal(string.Format("select * from test limit @{0},@{1}",PagedSqlStatement.SkipParameterName,PagedSqlStatement.TakeParameterName),sel);

       
        }


        [Fact]
        public void npgsql_test_pagination_strings()
        {
            var m = new PostgresProvider(Config.SqlServerProviderName);
            string cnt;
            string sel;
            m.MakePaged("select * from test", out sel, out cnt);
            Assert.Equal("select count(*) from test", cnt);
            Assert.Equal(string.Format("select * from test limit @{0} offset @{1}", PagedSqlStatement.TakeParameterName,PagedSqlStatement.SkipParameterName), sel);
            
        }

        [Fact]
        public void sqlserverce_pagination_strings()
        {
            var ce = new SqlServerCEProvider();
            string cnt;
            string sel;
            ce.MakePaged("select * from test", out sel, out cnt);
            Assert.Equal("select count(*) from test", cnt);
            Assert.Equal(string.Format("select * from test OFFSET @{0} ROWS FETCH NEXT @{1} ROWS ONLY", PagedSqlStatement.SkipParameterName, PagedSqlStatement.TakeParameterName), sel);
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}