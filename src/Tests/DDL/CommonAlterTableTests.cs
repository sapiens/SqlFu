using System.Data;
using SqlFu;
using SqlFu.DDL;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
    public abstract class CommonAlterTableTests:IDisposable
    {
        protected const string TableName = "users";
        private Stopwatch _t = new Stopwatch();
        protected DbAccess Db;
        protected IModifyTable Table;
        public CommonAlterTableTests()
        {
            Db = Setup.GetDb(engine: Engine);
            SetupTable();
            Table = Db.DatabaseTools.GetAlterTableBuilder(TableName);
        }

        void SetupTable()
        {
            var t = Db.DatabaseTools.GetCreateTableBuilder("users2", IfTableExists.DropIt);
            t.Columns.Add("Id", DbType.Int32, isNullable: false, autoIncrement: true);
            t.Constraints.AddPrimaryKeyOn("Id");
            t.ExecuteDDL();
            t = Db.DatabaseTools.GetCreateTableBuilder(TableName, IfTableExists.DropIt);
            t.Columns
                .Add("Id", DbType.Int32, isNullable: false, autoIncrement: false).AsPrimaryKey()
                .Add("Name", DbType.String, "30")
                .Add("Gender", DbType.Boolean, defaultValue: "1", isNullable: false)
                .Add("Email", DbType.String, "200", isNullable: false)
                .Add("Others", DbType.String)
                .Add("Number", DbType.Int32).WithCheck("Number>3", "ck_number").WithIndex("ix_users_others", false)
                .Add("ParentId", DbType.Int32, isNullable: false);
            t.Constraints.AddUniqueConstraintOn("Others", "uk_others")
             .IfDatabaseIs(DbEngine.MySql)
             .Redefine("KEY uk_others(Others(300))");
            t.Constraints["ck_number"].IfDatabaseIs(DbEngine.PostgreSQL).Redefine("constraint ck_number check (\"Number\">3)");
            t.ExecuteDDL();

        }

        protected abstract DbEngine Engine { get; }

        [Fact]
        public void add_two_uniques()
        {
            Table.Constraints.AddUniqueConstraintOn("Email", "uk_email_test").AddUniqueConstraintOn("Gender", "uk_gender");
            Table.ExecuteDDL();

            Assert.True(Db.DatabaseTools.ConstraintExists("uk_email_test"));
            Assert.True(Db.DatabaseTools.ConstraintExists("uk_gender"));
        }

        [Fact]
        public void drop_unique()
        {
            do_drop_unique();
            Assert.False(Db.DatabaseTools.ConstraintExists("uk_others"));
        }

        protected virtual void do_drop_unique()
        {
            Table.Constraints.Drop("uk_others").WithoutOptions()
                 .ExecuteDDL();
        }


        [Fact]
        public void drop_primary()
        {
            Table.Constraints.DropPrimaryKey().WithoutOptions().ExecuteDDL();
            //Table.Constraints.Drop("PK_users").WithoutOptions().ExecuteDDL();
            Assert.False(Db.DatabaseTools.TableHasPrimaryKey(TableName));
        }

        [Fact]
        public virtual void drop_check()
        {
            Table.Constraints.Drop("ck_number").WithoutOptions().ExecuteDDL();
            assert_drop_check();
        }

        protected virtual void assert_drop_check()
        {
            Assert.False(Db.DatabaseTools.ConstraintExists("ck_number"));
        }

        [Fact]
        public void drop_foreign_key()
        {
            Table.Constraints.AddForeignKeyOn("ParentId", "users2", "Id", keyName: "fk_users");
            Table.ExecuteDDL();
            Table = Db.DatabaseTools.GetAlterTableBuilder(TableName);
            Table.Constraints.Drop("fk_users").WithoutOptions().ExecuteDDL();
            Assert.False(Db.DatabaseTools.ConstraintExists("fk_users"));
        }


        [Fact]
        public void drop_index()
        {
            Table.Indexes.Drop("ix_users_others").WithoutOptions().ExecuteDDL();
            Assert.False(Db.DatabaseTools.IndexExists("ix_users_others", TableName));
        }

        [Fact]
        public void add_index_on_number()
        {
            Table.Indexes.AddIndexOn("Number", false, "ix_number");
            Table.ExecuteDDL();
            Assert.True(Db.DatabaseTools.IndexExists("ix_number", TableName));
        }

        [Fact]
        public void rename_column()
        {
            Table.Columns.Change("Name").RenameTo("Name1");
            Table.ExecuteDDL();
            Assert.True(Db.DatabaseTools.TableHasColumn(TableName,"Name1"));
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            Db.DatabaseTools.DropTable(TableName);
            Db.DatabaseTools.DropTable("users2");
        }

        #endregion
    }
}