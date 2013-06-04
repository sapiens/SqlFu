using SqlFu;
using SqlFu.DDL;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.DDL
{
    public abstract class CommonDatabaseToolsTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        protected DbAccess Db;
        protected bool DontDispose;
        
        public CommonDatabaseToolsTests()
        {
            Db = Setup.GetDb(engine:Engine);
            SetupTable();
        }

        protected IDatabaseTools Tools { get { return Db.DatabaseTools; } }

        protected abstract DbEngine Engine { get; }

        protected virtual void SetupTable()
        {
            Db.ExecuteCommand(@"
create table test
( id int not null,
name varchar(20),
constraint pk_test primary key (id)
);
create index ix_test on test(name)
");
        }

        [Fact]
        public void table_exists()
        {
            Assert.True(Tools.TableExists("test"));
        }


        [Fact]
        public void drop_table()
        {
            DontDispose = true;
            Tools.DropTable("test");
            Assert.False(Tools.TableExists("test"));
        }

        [Fact]
        public void rename_table()
        {
            Tools.RenameTable("test","test1");
            Assert.True(Tools.TableExists("test1"));
            Assert.False(Tools.TableExists("test"));
            Tools.RenameTable("test1", "test");
        }


        [Fact]
        public virtual void constraint_exists()
        {
            Assert.True(Tools.ConstraintExists("pk_test"));
        }

        [Fact]
        public void index_exists()
        {
            Assert.True(Tools.IndexExists("ix_test", "test"));
        }

        [Fact]
        public void column_exists()
        {
            Assert.True(Tools.TableHasColumn("test","name"));
        }


        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        #region Implementation of IDisposable

        public void Dispose()
        {
            if (DontDispose) return;
            Db.ExecuteCommand("drop table test");
        }

        #endregion
    }
}