using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using CavemanTools.Infrastructure;
using SqlFu.Migrations.Automatic;
using SqlFu.Migrations.Automatic.Models;

namespace SqlFu.Migrations
{
    public class DatabaseMigration : IConfigureMigrationsRunner
    {
        private readonly DbConnection _db;
        public const string DefaultSchemaName = "_GlobalSchema";
      
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

        public IConfigureMigrationsRunner SearchMigrations(params Assembly[] asm)
        {
            _asm.AddRange(asm);
            return this;
        }

        public IConfigureMigrationsRunner SearchAssemblyOf<T>()
        {
            _asm.Add(typeof (T).Assembly);
            return this;
        }

        //public IConfigureMigrationsRunner WithLogger(ILogWriter logger)
        //{
        //    logger.MustNotBeNull();
        //    _log = logger;
        //    return this;
        //}

        public IConfigureMigrationsRunner WithResolver(IResolveDependencies resolver)
        {
            resolver.MustNotBeNull();
            _resolver = resolver;
            return this;
        }

        public IConfigureMigrationsRunner WithResolver(Func<Type, object> resolver)
        {
            resolver.MustNotBeNull();
            _resolver=new DependencyContainerWrapper(resolver);
            return this;
        }

        public IManageMigrations Build()
        {
            if (_resolver == null) throw new InvalidOperationException("Missing dependency resolver");
            var types = _asm
                .SelectMany(a => a.GetTypesDerivedFrom<IMigrationTask>(true).Select(t => (IMigrationTask) _resolver.Resolve(t)))
                .Where(t => t.CurrentVersion != null)
                .ToArray();
            if (types.Length == 0)
            {
                throw new MigrationNotFoundException("None of the provided assemblies contained SqlFu migrations");
            }

            var runner = new MigrationTaskRunner(_db);

            return new MigrationsManager(GetSchemaExecutors(types, runner), runner);
        }

        public IAutomaticMigration BuildAutomaticMigrator()
        {
            SqlFuManager.Config.TableInfoFactory.Add(new TrackerMap().Info);
            var types = _asm
               .SelectMany(a => a.GetTypesDerivedFrom<IUninstallSchema>(true).Select(t => (IUninstallSchema)_resolver.Resolve(t)))
               .ToArray();
            return new AutomaticMigration(_db, Build(),types);
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
            BuildAutomaticMigrator().Install(schemas);
        }

        public void Uninstall(params string[] schemas)
        {
            BuildAutomaticMigrator().Uninstall(schemas);
        }
    }
}