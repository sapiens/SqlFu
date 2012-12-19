using System.Linq;
using System.Reflection;
using CavemanTools.Infrastructure;
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

        IManageMigrations BuildManager()
        {
            return DatabaseMigration.ConfigureFor(_db)
                .SearchAssembly(Assembly.GetExecutingAssembly())
                .WithLogger(new ConsoleLogger())
                .WithResolver(ActivatorContainer.Instance)
                .Build();
        }

        [Fact]
        public void two_schemas_found()
        {
            var manager = BuildManager();
            Assert.Equal(2,manager.Schemas.Count());
        }

        [Fact]
        public void testschema_has_three_tasks()
        {
            var test = BuildManager().GetSchemaMigrator("TestSchema") as SchemaMigrationExecutor;
            Assert.Equal(3, test.Tasks.Count());
        }

        [Fact]
        public void other_has_three_tasks()
        {
            var other = BuildManager().GetSchemaMigrator("Other") as SchemaMigrationExecutor;
            Assert.Equal(1, other.Tasks.Count());
        }

        [Fact]
        public void missing_migrations_throws()
        {
            Assert.Throws<MigrationNotFoundException>(
                ()=>DatabaseMigration.ConfigureFor(_db)
                .SearchAssembly(Assembly.GetCallingAssembly())
                .WithLogger(new ConsoleLogger())
                .WithResolver(ActivatorContainer.Instance)
                .Build());
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}