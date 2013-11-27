using System.ComponentModel.DataAnnotations;
using SqlFu;
using SqlFu.DDL;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
   public partial class User
    {
    
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime RegisteredAt { get; set; }
        
        public Guid? UserId { get; set; }
      
        public IfTableExists Options { get; set; }
      
        public string Bla { get; set; }

        public byte[] Data { get; set; }
      
        public string OK { get; set; }
    }


    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        
    }


    [Table("Users")]
    [Index("Email", Name = "ix_email")]
    [PrimaryKey("Id", AutoIncrement = true, Name = "PK_Users")]
    public class UserMetaData
    {
        [ForeignKey("ParentT", "ParentC", OnDelete = ForeignKeyRelationCascade.Cascade)]
        public string Name { get; set; }

        [RedefineFor(DbEngine.SqlServer, "bla")]
        [ColumnOptions(DefaultValue = "test", Size = "50")]
        public string Bla { get; set; }

        [ColumnOptions(IsNullable = true)]
        public string OK { get; set; }
    }


    public class FluentBuilderCreateTableTests
    {
        private Stopwatch _t = new Stopwatch();

        public FluentBuilderCreateTableTests()
        {

        }

        [Fact]
        public void test()
        {
            var sb = Setup.GetDb();
            var builder = sb.DatabaseTools.GetCreateTableBuilder<User>();
            var result = @"create table [Users] (
[Id] int NOT NULL IDENTITY(1,1),
[Name] nvarchar(max) NOT NULL,
[RegisteredAt] datetime NOT NULL,
[UserId] uniqueidentifier NULL,
[Options] int NOT NULL,
[Bla] bla,
[Data] varbinary(max) NOT NULL,
[OK] nvarchar(max) NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
 CONSTRAINT [FK_Users_ParentT_name] FOREIGN KEY ([Name]) REFERENCES [ParentT]([ParentC]) ON DELETE CASCADE ON UPDATE NO ACTION);
CREATE INDEX [ix_email] ON [Users] ([Email]);".Replace("\n", "\r\n");
 

            var sql = builder.GetSql();
            Assert.Equal(result,sql);
            Write(sql);
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}