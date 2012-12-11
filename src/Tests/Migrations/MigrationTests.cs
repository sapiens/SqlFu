using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using CavemanTools.Logging;
using Moq;
using SqlFu;
using SqlFu.Migrations;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
   [Migration("1.0.0","1.0.1")]
   public class Migration1:AbstractMigrationTask
   {
       private readonly StringBuilder _builder=new StringBuilder();

       public Migration1()
       {
           
       }
       public Migration1(StringBuilder builder)
       {
           _builder = builder;
       }

       public override void Execute(IAccessDb db)
       {
           _builder.Append("1");
       }
   }

    [Migration("1.0.2")]
    public class InstallerTask:AbstractMigrationTask
    {
        private readonly StringBuilder _sb=new StringBuilder();

        public InstallerTask()
        {
            
        }
        public InstallerTask(StringBuilder sb)
        {
            _sb = sb;
        }

        public override void Execute(IAccessDb db)
        {
            _sb.Append("1.0.2");
        }
    }
   
    [Migration("1.0.1","1.0.2")]
    public class Migration2:AbstractMigrationTask
    {
         private readonly StringBuilder _builder=new StringBuilder();

        public Migration2()
        {
            
        }
       public Migration2(StringBuilder builder)
       {
           _builder = builder;
       }

       public override void Execute(IAccessDb db)
       {
           _builder.Append("2");
       }
    }


    public class MigrationTests
    {
        private Stopwatch _t = new Stopwatch();
        private List<IMigrateDatabase> _tasks;
        private MigrationsRunner _runner;
        private StringBuilder _sb;
            
        class FakeTransaction:DbTransaction
        {
            #region Overrides of DbTransaction

            public override void Commit()
            {
                
            }

            public override void Rollback()
            {
                
            }

            protected override DbConnection DbConnection
            {
                get { throw new NotImplementedException(); }
            }

            public override IsolationLevel IsolationLevel
            {
                get { throw new NotImplementedException(); }
            }

            #endregion
        }

        public MigrationTests()
        {
            var db = new Mock<IAccessDb>();
            db.Setup(d => d.BeginTransaction(null)).Returns(new FakeTransaction());
            LogHelper.Register(new ConsoleLogger(), "Default");
            _sb = new StringBuilder();
            _tasks = new List<IMigrateDatabase>();
            _tasks.Add(new Migration2(_sb));
            _tasks.Add(new InstallerTask(_sb));            
            _tasks.Add(new Migration1(_sb));
            
            _runner = new MigrationsRunner(db.Object, _tasks, LogHelper.DefaultLogger);
        }

     
        [Fact]
        public void versioning_attribute()
        {
            var d = new Migration1(new StringBuilder());
            Assert.Equal("1.0.0",d.CurrentVersion);
            Assert.Equal("1.0.1",d.NextVersion);
        }

        [Fact]
        public void setup_latest_version()
        {
            _runner.InstallSchema();
            Assert.Equal("1.0.2",_sb.ToString());
        }



        [Fact]
        public void upgrade_from_101_To_102()
        {
            _runner.Migrate("1.0.1","1.0.2");
            Assert.Equal("2",_sb.ToString());
        }

        [Fact]
        public void upgrade_from_100_to_102()
        {
            _runner.MigrateToLatestFrom("1.0.0");
            Assert.Equal("12",_sb.ToString());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}