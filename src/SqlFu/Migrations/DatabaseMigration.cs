using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using SqlFu.Migrations.Automatic;

namespace SqlFu.Migrations
{
    public class DatabaseMigration : IConfigureMigrationsRunner
    {
        private readonly DbConnection _db;
        public const string DefaultSchemaName = "_GlobalSchema";
        private ILogWriter _log = NullLogger.Instance;
        private readonly List<Assembly> _asm = new List<Assembly>();
        private IResolveDependencies _resolver = ActivatorContainer.Instance;

        public DatabaseMigration(DbConnection db)
        {
            _db = db;
        }

        public static IConfigureMigrationsRunner ConfigureFor(DbConnection db)
        {
            return new DatabaseMigration(db);
        }

        public IConfigureMigrationsRunner SearchAssembly(params Assembly[] asm)
        {
            _asm.AddRange(asm);
            return this;
        }

        public IConfigureMigrationsRunner SearchAssemblyOf<T>()
        {
            _asm.Add(typeof (T).Assembly);
            return this;
        }

        public IConfigureMigrationsRunner WithLogger(ILogWriter logger)
        {
            logger.MustNotBeNull();
            _log = logger;
            return this;
        }

        public IConfigureMigrationsRunner WithResolver(IResolveDependencies resolver)
        {
            resolver.MustNotBeNull();
            _resolver = resolver;
            return this;
        }

        public IManageMigrations Build()
        {
            if (_resolver == null) throw new InvalidOperationException("Missing dependency resolver");
            var types = _asm
                .SelectMany(a => AssemblyExtensions.GetTypesImplementing<IMigrationTask>(a, true)
                                                   .Select(t => (IMigrationTask) _resolver.Resolve(t)))
                .Where(t => t.CurrentVersion != null)
                .ToArray();
            if (types.Length == 0)
            {
                throw new MigrationNotFoundException("None of the provided assemblies contained SqlFu migrations");
            }

            var runner = new MigrationTaskRunner(_db, _log);

            return new MigrationsManager(GetSchemaExecutors(types, runner), runner);
        }

        public IAutomaticMigration BuildAutomaticMigrator()
        {
            return new AutomaticMigration(_db, Build(), _log);
        }

        private IEnumerable<IMigrateSchema> GetSchemaExecutors(IEnumerable<IMigrationTask> tasks, IRunMigrations runner)
        {
            var groups = tasks.GroupBy(t => t.SchemaName);
            foreach (var group in groups)
            {
                yield return new SchemaMigrationExecutor(runner, group, group.Key);
            }
        }

        public void PerformAutomaticMigrations(params string[] schemas)
        {
            BuildAutomaticMigrator().Execute(schemas);
        }
    }
}