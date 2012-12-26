using SqlFu;
using SqlFu.DDL;
using SqlFu.DDL.Internals;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
    [Table("Users")]
    [Index("Email")]
    public class User
    {
    
        public int Id { get; set; }
        
        
        [ForeignKey("ParentT","ParentC",OnDelete = ForeignKeyRelationCascade.Cascade)]
        public string Name { get; set; }
        
        public DateTime RegisteredAt { get; set; }
        
        public Guid? UserId { get; set; }
        //[InsertAsString]
        public IfTableExists Options { get; set; }
        [RedefineFor(DbEngine.SqlServer, "bla")]
        [ColumnOptions(DefaultValue = "test",Size = "50")]
        public string Bla { get; set; }

        public byte[] Data { get; set; }
        [ColumnOptions(IsNullable = true)]
        public string OK { get; set; }
    }
    
    public class FluentBuilderCreateTableTests
    {
        private Stopwatch _t = new Stopwatch();

        public FluentBuilderCreateTableTests()
        {

        }

        //[Fact]
        public void test()
        {
            var sb = Setup.GetDb();
            var builder = sb.DatabaseTools.GetCreateTableBuilder<User>();
            Write(builder.GetSql());
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}