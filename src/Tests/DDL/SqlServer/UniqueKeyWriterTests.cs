using System.Text;
using SqlFu;
using SqlFu.DDL.Generators;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.SqlServer
{
    public class UniqueKeyWriterTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private UniqueKeyConstraint _key;
        private SqlServerUniqueKeyWriter _writer;

        public UniqueKeyWriterTests()
        {
            _sb = new StringBuilder();
            _key = new UniqueKeyConstraint() {Columns = "id, user name",Name = "PK_Id",IsPrimary = true};
            _writer = new SqlServerUniqueKeyWriter(_sb);
        }

        [Fact]
        public void primary_key_no_options()
        {
            var pk = new SqlServerUniqueKeyWriter(_sb);
            
            pk.Write(_key);
            Assert.Equal(" CONSTRAINT [PK_Id] PRIMARY KEY ([id],[user name])",_sb.ToString());
        }
        
        [Fact]
        public void primary_key_clustered()
        {
            var pk = new SqlServerUniqueKeyWriter(_sb);
            _key.Options.Add(SqlServerOptions.PrimaryKeyIs.Clustered());
            pk.Write(_key);
            Assert.Equal(" CONSTRAINT [PK_Id] PRIMARY KEY CLUSTERED ([id],[user name])",_sb.ToString());
        }
        
        [Fact]
        public void primary_key_non_clustered()
        {
            var pk = new SqlServerUniqueKeyWriter(_sb);
            _key.Options.Add(SqlServerOptions.PrimaryKeyIs.NonClustered());
            pk.Write(_key);
            Assert.Equal(" CONSTRAINT [PK_Id] PRIMARY KEY NONCLUSTERED ([id],[user name])",_sb.ToString());
        }

        [Fact]
        public void unique_key()
        {
            _key.IsPrimary = false;
            _writer.Write(_key);
            Assert.Equal(" constraint [pk_id] unique ([id],[user name])",_sb.ToString().ToLowerInvariant());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}