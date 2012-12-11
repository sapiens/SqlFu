using System.Linq;
using System.Reflection;
using CavemanTools.Logging;
using Moq;
using SqlFu;
using SqlFu.Migrations;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class MIgrationsConfigTests
    {
        private Stopwatch _t = new Stopwatch();
        private IAccessDb _db;

        public MIgrationsConfigTests()
        {
            var db = new Mock<IAccessDb>();
            _db = db.Object;
        }

        [Fact]
        public void migration_config()
        {
            var runner=DatabaseMigration.ConfigureFor(_db)
                .SearchAssembly(Assembly.GetExecutingAssembly())
                .UseLogger(new ConsoleLogger()).Build();
            Assert.Equal(3,(runner as MigrationsRunner).Tasks.Count());
        }

        [Fact]
        public void missing_migrations_throws()
        {
            Assert.Throws<MigrationNotFoundException>(
                ()=>DatabaseMigration.ConfigureFor(_db)
                .SearchAssembly(Assembly.GetCallingAssembly())
                .UseLogger(new ConsoleLogger()).Build());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}