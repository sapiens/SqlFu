using SqlFu;
using SqlFu.Migrations;
using Tests.Migrations.Data;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
   
    public class MigrationMiscTests
    {
        private Stopwatch _t = new Stopwatch();

        public MigrationMiscTests()
        {

        }

        [Fact]
        public void task_processes_attribute()
        {
            var task = new InstallerTask();
            Assert.Equal("1.0.2",task.CurrentVersion.ToString());
            Assert.Equal(3,task.Priority);
            Assert.Equal("TestSchema",task.SchemaName);
        }

    

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}