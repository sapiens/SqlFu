using System.Data;
using System.Text;
using SqlFu;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.SqlServer
{
    public class ColumnWriterTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private ColumnDefinition _col;
        private SqlServerColumnWriter _cw;

        public ColumnWriterTests()
        {
            _sb = new StringBuilder();
            _col = new ColumnDefinition();
            _col.Name = "User Id";
            _col.Type=DbType.Boolean;
            _cw = new SqlServerColumnWriter(_sb);
        }

        [Fact]
        public void column_not_null_with_default_no_options()
        {
            var w = new SqlServerColumnWriter(_sb);
            _col.IsNullable = false;
            _col.DefaultValue = "0";
            w.Write(_col);
            Assert.Equal("[User Id] bit NOT NULL DEFAULT 0",_sb.ToString());
        }

        [Fact]
        public void column_with_identity_and_options()
        {
            var w = new SqlServerColumnWriter(_sb);
            _col.IsIdentity = true;
            _col.IsNullable = true;
            _col.Options.Add(SqlServerOptions.ColumnIs.Sparse());
            w.Write(_col);
            Assert.Equal("[User Id] bit SPARSE NULL IDENTITY(1,1)", _sb.ToString());
        }

        [Fact]
        public void varchar_column_with_collation()
        {
            _col.Type=DbType.AnsiString;
            _col.Size = "12";
            _col.Collate = "Latin_General_CI_AI";
            _cw.Write(_col);

            Assert.Equal("[User Id] varchar(12) COLLATE Latin_General_CI_AI NOT NULL",_sb.ToString());
        }

        [Fact]
        public void nchar_no_size_with_default()
        {
            _col.Type=DbType.StringFixedLength;
            _col.DefaultValue = "test";
            _cw.Write(_col);

            Assert.Equal("[User Id] nchar(1) NOT NULL DEFAULT 'test'",_sb.ToString());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}