using System.Data;
using SqlFu;
using SqlFu.DDL;
using SqlFu.DDL.Generators.SqlServer;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL.SqlServer
{
    public class CreateTableTests:CommonCreateTableTests
    {
        private Stopwatch _t = new Stopwatch();
        private SqlServerDDLWriter _ddl;
        private SqlFuConnection db;

        public CreateTableTests()
        {
            db = Db;
            _ddl = new SqlServerDDLWriter(db);
            db.DatabaseTools.DropTable("test");
        }

        
        public void test()
        {
            var table = new CreateTableBuilder(Config.GetDb(),_ddl,"bla", IfTableExists.Ignore);
            table.Columns
                .Add("Id", DbType.Int32, isNullable: false, autoIncrement: true)
                .Add("Name", DbType.String, size: "50")
                    .IfDatabaseIs(DbEngine.SqlServer).SetColumnOptions(SqlServerOptions.ColumnIs.Sparse())
                .Add("ParentId",DbType.Int32,isNullable:false)
                .Add("Uid",DbType.Guid)
                ;

            table.Constraints.AddPrimaryKeyOn("Id")
                .IfDatabaseIs(DbEngine.SqlServer)
                .PrimaryKeyOptions(SqlServerOptions.PrimaryKeyIs.Clustered());
                
            table.Constraints.AddForeignKeyOn("ParentId", "users", "Id", onDelete: ForeignKeyRelationCascade.Cascade);
            table.Constraints.AddCheck("Name like 'a%'","CK_bla_name");
            table.Indexes.AddIndexOn("Name", true, "ix_name")
                .IfDatabaseIs(DbEngine.SqlServer)
                .SetOptions(SqlServerOptions.Index.NonClustered());
                
            var s = table.GetSql();
            Write(s);          
        }

       
        protected override DbEngine Engine
        {
            get { return DbEngine.SqlServer;}
        }

        [Fact]
        public override void custom_constraints()
        {
            var t = db.DatabaseTools.GetCreateTableBuilder("bla", IfTableExists.DropIt);
            t.Columns.Add("primary", DbType.Int32, isNullable: false).AsPrimaryKey();
            t.Columns.Add("Id", DbType.String, "30");
            t.Constraints.IfDatabaseIs(DbEngine.SqlServer).AddConstraint("constraint uq_tr unique([id])");
            t.ExecuteDDL();
            Assert.True(db.DatabaseTools.ConstraintExists("uq_tr"));
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
     
    }
}