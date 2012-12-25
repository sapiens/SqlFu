using System;
using System.Data;
using System.Data.Common;
using SqlFu;
using SqlFu.DDL;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using Xunit;

namespace Tests.DDL.SqlServer
{
    public class AlterTableColumnTests:IDisposable
    {
        private DbAccess _db;
        private SqlServerDDLWriter _writer;
        private ModifyTableBuilder _table;

        public AlterTableColumnTests()
        {
            _db = Setup.GetDb();
            _writer = new SqlServerDDLWriter(_db);
            _table = new ModifyTableBuilder(_db, _writer, "users");
            SetupTestTable();
        }

        void SetupTestTable()
        {
            IModifyColumns c;
            

            //_db.ExecuteCommand(
            //    "create table users (Id int not null,Name nvarchar(30) collate Latin1_General_CI_AI not null,Gender bit default 1, Email varchar(Max) sparse null" +
            //    ", constraint [pk_users_id] primary key (Id))");
            var t = _db.DatabaseTools.GetCreateTableBuilder("users",IfTableExists.DropIt);
            t.Columns
                .Add("Id", DbType.Int32, isNullable: false,autoIncrement:true).AsPrimaryKey("pk_users")
                .Add("Name", DbType.String, "30",collation: "Latin1_General_CI_AI")
                .Add("Gender", DbType.Boolean, defaultValue: "1", isNullable: false)
                .Add("Email",DbType.String,"max")
                .Add("Others",DbType.String,"20")
                .IfDatabaseIs(DbEngine.SqlServer)
                .SetColumnOptions(SqlServerOptions.ColumnIs.Sparse());
            t.Indexes.AddIndexOn("Others",false,"ix_users_others");
            t.ExecuteDDL();
        }

        [Fact]
        public void rename_column()
        {
            _table.Columns.RenameColumn("Name", "FullName");
            _table.ExecuteDDL();
            Assert.True(_db.DatabaseTools.TableHasColumn("users","FullName"));
            Assert.False(_db.DatabaseTools.TableHasColumn("users","Name"));

            //Assert.DoesNotThrow(()=>_db.Fetch<string>("select FullName from users"));
            //Assert.Equal("EXEC sp_rename 'users.Name', 'FullName', 'COLUMN'", GetSql());
        }

        [Fact]
        public void add_column_and_set_name_not_nullable()
        {
           _table.Columns.Add("TestColumn", DbType.Int32).Add("second",DbType.Guid).IfDatabaseIs(DbEngine.SqlServer).SetColumnOptions(SqlServerOptions.ColumnIs.Sparse());
            _table.Columns.Change("Name").SetNullable(false);
            _table.Columns.Change("Email").SetNullable(false);
            Assert.DoesNotThrow(()=>_table.ExecuteDDL());
            
        }

        [Fact]
        public void change_gender_to_varchar()
        {
            _table.Columns.Change("Gender").SetType(DbType.String, "1").SetDefault("M");
            _table.ExecuteDDL();
            Assert.DoesNotThrow(()=>_db.Insert("users",new {Gender="F"}));
        }

        [Fact]
        public void change_collation()
        {
            _table.Columns.Change("Name").SetCollation("Latin1_General_CS_AS");
            Assert.Equal("alter table [users] alter column [Name] nvarchar(30) collate Latin1_General_CS_AS;".ToLowerInvariant(), _table.GetSql().Trim().ToLowerInvariant());
        }

        //[Fact]
        public void drop_default()
        {
            _table.Columns.Change("Gender").DropDefault();          
            Write(_table.GetSql());
        }

        [Fact]
        public void drop_column_with_default()
        {
            _table.Columns.Drop("gender").WithoutOptions().ExecuteDDL();
            Assert.False(_db.DatabaseTools.TableHasColumn("users","gender"));
            //var exceptionflag = false;
            //try
            //{
            //    _db.ExecuteCommand("select Gender from users");
            //}
            //catch (DbException ex)
            //{
            //    if (ex.Message.ToLowerInvariant().Contains("invalid column")) exceptionflag = true;               
            //}
            //Assert.True(exceptionflag);
        }

        [Fact]
        public void drop_column_and_index()
        {
            _table.Columns.Drop("Others").WithoutOptions()
                .Indexes.Drop("ix_users_others");
            _table.ExecuteDDL();
            Assert.False(_db.DatabaseTools.TableHasColumn("users", "Others"));
            Assert.False(_db.DatabaseTools.IndexExists("ix_users_others", "users"));
        }

        [Fact]
        public void drop_constraint()
        {
           _table.Constraints.Drop("pk_users").WithoutOptions().ExecuteDDL();
            Assert.False(_db.DatabaseTools.ConstraintExists("pk_users"));
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
        public void Dispose()
        {
            _db.DatabaseTools.DropTable("users");
        }
    }
}