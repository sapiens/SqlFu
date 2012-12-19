using System.Collections.Generic;
using System.Linq;
using SqlFu.Migrations;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class MigrationsManagerTests
    {
        private Stopwatch _t = new Stopwatch();
        private MigrationsManager _manager;
        private List<IMigrateSchema> _schemas;

        class FakeSchemaMigrator:IMigrateSchema
        {
            /// <summary>
            /// Gets or sets the runner used to execute tasks
            /// </summary>
            public IRunMigrations Runner { get; set; }
            public string SchemaName { get;  set; }
            public void Migrate(string currentVersion, string nextVersion)
            {
                throw new NotImplementedException();
            }

            public void MigrateToLatestFrom(string currentVersion)
            {
                throw new NotImplementedException();
            }

            public string LatestVersionAvailable { get;  set; }
            public int Priority { get; set; }
            public void InstallSchema()
            {
                Console.WriteLine(SchemaName);
            }
        }

        public MigrationsManagerTests()
        {
            _schemas = new List<IMigrateSchema>();
            _schemas.Add(new FakeSchemaMigrator(){SchemaName = "Test",Priority = 1});
            _schemas.Add(new FakeSchemaMigrator() { SchemaName = "Test1", Priority = 4 });
            _schemas.Add(new FakeSchemaMigrator(){SchemaName = "Test3",Priority = 1});

            _manager = new MigrationsManager(_schemas, new FakeRunner());
        }

        [Fact]
        public void schemas_are_ordered_by_priority()
        {
            Assert.Equal(4,_manager.Schemas.First().Priority);
            _manager.InstallAllSchemas();
        }

        [Fact]
        public void get_schema_executor()
        {
            Assert.Equal("Test1",_manager.GetSchemaMigrator("Test1").SchemaName);
        }

        [Fact]
        public void adding_a_new_schema_is_ordered()
        {
            _manager.Add(new FakeSchemaMigrator(){SchemaName = "bla",Priority = 300});
            Assert.Equal("bla",_manager.Schemas.First().SchemaName);
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}