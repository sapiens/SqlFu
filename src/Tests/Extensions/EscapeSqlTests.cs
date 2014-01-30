using System;
using System.Diagnostics;
using SqlFu;
using Xunit;

namespace Tests.Extensions
{
    public class EscapeSqlTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private SqlFuConnection _db;


        public EscapeSqlTests()
        {
            _db = Config.GetDb();
        }

        [Fact]
        public void escape_sql_with_comma__or_space_delimiter()
        {
            var sql = "select $id,$name from dbo.$table";
            Assert.Equal("select [id],[name] from dbo.[table]",_db.EscapeSql(sql));
        }
        
        [Fact]
        public void escape_sql_with_assignment_delimiter()
        {
            var sql = "select $Data from $table where $Type=@0 order by $LastUpdate";
            Assert.Equal("select [Data] from [table] where [Type]=@0 order by [LastUpdate]",_db.EscapeSql(sql));
        }

        [Fact]
        public void escape_sql_with_tabs_or_newlines_delimiters()
        {
            var sql = @"select $id
,$name from dbo.$table
where $id>@0";
            Assert.Equal(@"select [id]
,[name] from dbo.[table]
where [id]>@0", _db.EscapeSql(sql));
        }

        [Fact]
        public void escape_sql_with_postgres_cast()
        {
            var sql = "select $id::int,$name from dbo.$table";
            Assert.Equal("select [id]::int,[name] from dbo.[table]", _db.EscapeSql(sql));
        }

        [Fact]
        public void escape_sql_with_paranthesis()
        {
            var sql = "select $id,$name from dbo.$table where ($id)";
            Assert.Equal("select [id],[name] from dbo.[table] where ([id])", _db.EscapeSql(sql));
        }

        [Fact]
        public void escape_sql_with_literal_escape()
        {
            var sql = "select $id,$name from dbo.$table where $id regex('$$')";
            Assert.Equal("select [id],[name] from dbo.[table] where [id] regex('$')", _db.EscapeSql(sql));
        }

        protected void Write(object format, params object[] param)
        {
            if (param.Length == 0)
            {
                Console.WriteLine(format);
            }
            else Console.WriteLine(format.ToString(), param);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Config.CleanUp();
        }
    }
}