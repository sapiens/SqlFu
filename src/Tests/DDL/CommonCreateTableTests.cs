using System.Data;
using System.Data.Common;
using SqlFu;
using SqlFu.DDL;
using SqlFu.DDL.Generators.MySql;
using SqlFu.DDL.Generators.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
    public abstract class CommonCreateTableTests:IDisposable
    {
        protected const string TableName = "abf_test";
        private Stopwatch _t = new Stopwatch();
        protected SqlFuConnection Db;
        protected ICreateTable Table;
        protected bool DontDispose;
      

        public CommonCreateTableTests()
        {
            Db = Setup.GetDb(engine: Engine);
            SetupTable();
        }

        protected abstract DbEngine Engine { get;}

        internal void SetupTable()
        {
            Table = Db.DatabaseTools.GetCreateTableBuilder(TableName,IfTableExists.Throw);
            Table.Columns
                .Add("Id", DbType.Int32, isNullable: false, autoIncrement: true).AsPrimaryKey("pk_test")
                .Add("Name", DbType.String, size: "50").AsUnique("uk_name")//.WithCheck("Name like '%a'","ck_name")+            	
            	.Add("Uid", DbType.Guid)
            	.Add("Int32", DbType.Int32)
            	.Add("Int64", DbType.Int64)
            	.Add("Int16", DbType.Int16)
            	.Add("UInt16", DbType.UInt16)
	          	.Add("UInt32", DbType.UInt32)
            	.Add("UInt64", DbType.UInt64)
	           	.Add("Single", DbType.Single)
            	.Add("Double", DbType.Double)
            	.Add("DateTime", DbType.DateTime)
            	.Add("DateTimeOffset", DbType.DateTimeOffset);

            Table.TableOptionsFor(DbEngine.MySql, MySqlOptions.Table.AutoIncrementValueIs(1),MySqlOptions.Table.EngineIs(TableEngineType.InnoDb));
            Table.Constraints.AddCheck("Name like '%a'", "ck_name")
                 .IfDatabaseIs(DbEngine.PostgreSQL).Redefine("constraint ck_name check(\"Name\" like '%a')");
        }

        [Fact]
        public void create_table_doesnt_throw()
        {
           Assert.DoesNotThrow(()=>Table.ExecuteDDL());
        }

        [Fact]
        public void identity_is_set()
        {
            Table.ExecuteDDL();
            Assert.Equal(1,Db.Insert(TableName, new {Name = "ha"}).InsertedId<int>());
        }

        [Fact]
        public virtual void primary_key_is_defined()
        {
            Table.ExecuteDDL();
            assert_primary_key_exists();
        }

        protected virtual void assert_primary_key_exists()
        {
            Assert.True(Db.DatabaseTools.ConstraintExists("pk_test"));
        }

        [Fact]
        public void unique_key_is_defined()
        {
            Table.ExecuteDDL();
            assert_unique_key();
        }

        protected virtual void assert_unique_key()
        {
            Assert.True(Db.DatabaseTools.ConstraintExists("uk_name"));
        }

        [Fact]
        public void check_is_defined()
        {
            Table.ExecuteDDL();
            assert_check();
        }

        protected virtual void assert_check()
        {
            Assert.True(Db.DatabaseTools.ConstraintExists("ck_name"));            
        }

        [Fact]
        public void foreign_key_is_defined()
        {
            var parent = Db.DatabaseTools.GetCreateTableBuilder("ptest",IfTableExists.DropIt);
            parent.Columns.Add("Id", DbType.Int32, isNullable: false).AsPrimaryKey();
            parent.ExecuteDDL();
            Table.Columns.Add("parent", DbType.Int32).IsForeignKeyFrom("ptest", "Id", keyName: "fk_ptest_test");
            Table.ExecuteDDL();
            assert_foreign_key();
            Db.DatabaseTools.DropTable(TableName);
            
           Db.DatabaseTools.DropTable("ptest");
            DontDispose = true;

        }

        protected virtual void assert_foreign_key()
        {
            Assert.True(Db.DatabaseTools.ConstraintExists("fk_ptest_test"));
        }

        public abstract void custom_constraints();


        [Fact]
        public void create_if_exists_throw()
        {
            Table.ExecuteDDL();
            Assert.Throws<TableExistsException>(() => Table.ExecuteDDL());

        }

        [Fact]
        public void create_if_exists_drop()
        {
            Table.ExecuteDDL();
            Assert.True(Db.DatabaseTools.TableExists(TableName));
            var ddl = Db.DatabaseTools.GetCreateTableBuilder(TableName, IfTableExists.DropIt);
            ddl.Columns.Add("sid", DbType.Int32);
            Assert.DoesNotThrow(() => ddl.ExecuteDDL());
            Assert.True(Db.DatabaseTools.TableHasColumn(TableName,"sid"));
        }

        [Fact]
        public void create_if_exists_ignore()
        {
            Table.ExecuteDDL();
            Db.Insert(TableName, new { Name = "aha", Uid = Guid.NewGuid() });
            var ddl = Db.DatabaseTools.GetCreateTableBuilder(TableName, IfTableExists.Ignore);
            ddl.Columns.Add("sid", DbType.Int32);
            Assert.DoesNotThrow(() => ddl.ExecuteDDL());
            var cnt = Db.QuerySingle<dynamic>("select * from " + Db.Provider.EscapeName(TableName));
            Assert.Equal("aha", cnt.Name);
        }


        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
       
            if (!DontDispose) Db.DatabaseTools.DropTable(TableName); 
            Db.Dispose();
        }

        #endregion
    }
}