using System.Text;
using SqlFu;
using SqlFu.DDL.Generators.MySql;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.MySql
{
    public class MySqlColumnWriterTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private MySqlColumnWriter _writer;
        private ColumnDefinition _col;

        public MySqlColumnWriterTests()
        {
            _sb = new StringBuilder();
            _writer = new MySqlColumnWriter(_sb);
            _col = new ColumnDefinition();
        }

        [Fact]
        public void redefine_column_as_enum()
        {
            _col.Name = "test";
            _col.Redefine(DbEngine.MySql, "enum('a','b')");
            _writer.Write(_col);
            Assert.Equal("`test` enum('a','b')",_sb.ToString());
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}