#if MySql
using System.Data;
using MySql.Data.MySqlClient;
using SqlFu;
using SqlFu.DDL.Generators.MySql;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.MySql
{
    public class MySqlCreateTableTests:CommonCreateTableTests
    {
        private Stopwatch _t = new Stopwatch();

        public MySqlCreateTableTests()
        {

        }

        [Fact]
        public void table_options()
        {
            Table.TableOptionsFor(DbEngine.MySql, MySqlOptions.Table.EngineIs(TableEngineType.Memory),
                                  MySqlOptions.Table.AutoIncrementValueIs(2));
            Table.Indexes.AddIndexOn("Uid", true).IfDatabaseIs(DbEngine.MySql).SetOptions(MySqlOptions.Index.IsType(MySqlIndexType.HASH));
            Write(Table.GetSql());
        }

        protected override DbEngine Engine
        {
            get { return DbEngine.MySql;}
        }
        protected override void assert_primary_key_exists()
        {
            Db.Insert(TableName, new {Name = "bla"});
            Db.Insert(TableName, new {Name = "bla2"});
            Assert.Equal(2,Db.GetValue<int>("select max(id) from "+TableName));
        }

        protected override void assert_unique_key()
        {
            Db.Insert(TableName, new { Name = "bla" });
            Assert.Throws<MySqlException>(() => Db.Insert(TableName, new {Name = "bla"}));
        }

        protected override void assert_check()
        {
            Write("MySql Ignores checks");
        }

        protected override void assert_foreign_key()
        {
            Db.Insert("ptest", new { id = 2 });
            Db.Insert(TableName, new {parent = 2});
            Assert.Throws<MySqlException>(()=>Db.Insert(TableName, new {parent = 5}));
        }

        [Fact]
        public void redefine_column()
        {
            Table.Columns.Add("TType", DbType.String, "30")
                 .IfDatabaseIs(DbEngine.MySql)
                 .RedefineColumnAs("enum('a','b')");
            Table.ExecuteDDL();
            Assert.True(Db.DatabaseTools.TableHasColumn(TableName,"TType"));
        }

        [Fact]
        public override void custom_constraints()
        {
            Table.Constraints.IfDatabaseIs(DbEngine.MySql).AddConstraint("unique ix_uid (uid)");
            Table.ExecuteDDL();
            Db.Insert(TableName, new {Uid = "b"});
            Assert.Throws<MySqlException>(()=>Db.Insert(TableName, new {Uid = "b"}));
        }
    }
}
#endif