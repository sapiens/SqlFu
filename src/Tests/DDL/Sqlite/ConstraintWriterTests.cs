using System.Text;
using SqlFu;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Generators.Sqlite;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.Sqlite
{
    public class SqliteConstraintWriterTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private ConstraintDefinition _key;

        public SqliteConstraintWriterTests()
        {
            _sb = new StringBuilder();
            _key = new UniqueKeyConstraint() {Columns = "id, user name",Name = "PK_Id"};
        }


        [Fact]
        public void FactMethodName()
        {
            var arr = new int[1];
            arr[0] = 1;
            var arr2 = arr;
            arr2[0]++;
            Write(arr[0].ToString());
        }

        //[Fact(Skip = "not impl")]
        //public void primary_key_no_options()
        //{
        //    var pk = new SqliteUniqueKeyWriter(_sb);
        //    pk.Write(_key);
        //    Assert.Equal(" CONSTRAINT PK_Id PRIMARY KEY (\"id\",\"user name\")",_sb.ToString());
        //}
               

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}