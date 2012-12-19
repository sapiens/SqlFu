using System.Collections.Generic;
using System.Text;
using CavemanTools.Logging;
using Moq;
using SqlFu;
using SqlFu.Migrations;
using Tests.Migrations.Data;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class MigrationTaskRunnerTests
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private MigrationTaskRunner _runner;
        private readonly Mock<IAccessDb> _db;

        public MigrationTaskRunnerTests()
        {
            _db = new Mock<IAccessDb>();
            _db.Setup(d => d.BeginTransaction(null)).Returns(new FakeTransaction());
            LogHelper.Register(new ConsoleLogger(), "Default");
            _sb = new StringBuilder();
           _runner = new MigrationTaskRunner(_db.Object, LogHelper.DefaultLogger);
        }

        [Fact]
        public void run_task()
        {
            var m = new Migration1(_sb);
            _runner.Run(m);
            Assert.Equal("1",_sb.ToString());
        }

        [Fact]
        public void run_multiple_tasks()
        {
            var m = new Migration1(_sb);
            _runner.Run(m, new Migration2(_sb));
            Assert.Equal("12", _sb.ToString());
        }

        [Fact]
        public void tasks_are_wrapped_in_a_transaction()
        {
            _runner.Run(new Migration2(_sb),new Migration1(_sb));
            _db.Verify(d=>d.BeginTransaction(null),Times.Once());
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}