#if Sqlite
using System.Data.SQLite;
using SqlFu;
using Xunit;

namespace Tests.DDL.Sqlite
{
    public class SqliteCreateTableTests:CommonCreateTableTests
    {
        protected override DbEngine Engine
        {
            get { return DbEngine.SQLite;}
        }
        //[Fact]
        public override void custom_constraints()
        {
            throw new System.NotImplementedException();
        }

        [Fact(Skip = "Primary key is automatically defined with identity")]
        public override void primary_key_is_defined()
        {
            
        }

        protected override void assert_check()
        {
            Assert.Throws<SQLiteException>(() => Db.Insert("test", new { Name = "test" }));
        }

        
        protected override void assert_foreign_key()
        {
            Db.Insert("ptest", new {Id = 1},false);
            Db.Insert(TableName, new {parent = 1});
            //needs foreign keys to be enabled, by default sqlite does not enforce it
           Assert.Throws<SQLiteException>(() => Db.Insert("test", new {parent = 2}));
        }

        protected override void assert_unique_key()
        {
            
            Db.Insert(TableName, new {Name = "aha"});
            Assert.Throws<SQLiteException>(() => Db.Insert("test", new {Name = "aha"}));
        }
    }
}
#endif