using System.Text;
using SqlFu;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.SqlServer
{
    public class IndexWriterTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private IndexDefinition _idx;
        private SqlServerIndexWriter _iw;

        public IndexWriterTests()
        {
            _sb = new StringBuilder();
            _idx = new IndexDefinition("table");
            _idx.Name = "ix_bla";
            _idx.Columns.AddColumns("name");
            _iw = new SqlServerIndexWriter(_sb);
        }

        [Fact]
        public void index_with_includes_and_options()
        {
            _idx.Options.Add(SqlServerOptions.Index.Include("col1","col 2"));
            _idx.Options.Add(SqlServerOptions.Index.Clustered());
            _idx.Options.Add(SqlServerOptions.Index.IgnoreDupKey(true));
            _idx.Options.Add(SqlServerOptions.Index.DropExisting(false));
            
            _iw.Write(_idx);

            Assert.Equal("CREATE CLUSTERED INDEX [ix_bla] ON [table] ([name]) INCLUDE ([col1],[col 2]) WITH (IGNORE_DUP_KEY=ON, DROP_EXISTING=OFF)",_sb.ToString());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}