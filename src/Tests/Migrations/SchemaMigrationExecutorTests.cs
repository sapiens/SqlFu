using System.Collections.Generic;
using System.Text;
using CavemanTools.Logging;
using SqlFu.Migrations;
using SqlFu.Migrations.Automatic;
using Tests.Migrations.Data;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class SchemaMigrationExecutorTests
    {
        private Stopwatch _t = new Stopwatch();
        private List<IMigrationTask> _tasks;
        private SchemaMigrationExecutor _schema;
        private StringBuilder _sb;
            
       

        public SchemaMigrationExecutorTests()
        {
            LogHelper.Register(new ConsoleLogger(), "Default");
            _sb = new StringBuilder();
            _tasks = new List<IMigrationTask>();
            _tasks.Add(new Migration2(_sb));
            _tasks.Add(new InstallerTask(_sb));            
            _tasks.Add(new Migration1(_sb));
            var runner = new FakeRunner();

            _schema = new SchemaMigrationExecutor(runner, _tasks,"TestSchema");
        }

        [Fact]
        public void two_versions_of_setup_Db_throws()
        {
            _tasks.Add(new InstallerTask());
            var s = new SchemaMigrationExecutor(new FakeRunner(), _tasks, "TestSchema");
            Assert.Throws<InvalidOperationException>(() => s.InstallSchema());
        }
     
        [Fact]
        public void versioning_attribute()
        {
            var d = new Migration1(new StringBuilder());
            Assert.Equal("1.0.0",d.CurrentVersion);
            Assert.Equal("1.0.1",d.NextVersion);
        }

        [Fact]
        public void latest_version()
        {
            Assert.Equal("1.0.2",_schema.LatestVersionAvailable);
        }

        [Fact]
        public void setup_latest_version()
        {
            _schema.InstallSchema();
            Assert.Equal("1.0.2",_sb.ToString());
        }



        [Fact]
        public void upgrade_from_101_To_102()
        {
            _schema.Migrate("1.0.1","1.0.2");
            Assert.Equal("2",_sb.ToString());
        }

        [Fact]
        public void upgrade_from_100_to_102()
        {
            _schema.MigrateToLatestFrom("1.0.0");
            Assert.Equal("12",_sb.ToString());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}