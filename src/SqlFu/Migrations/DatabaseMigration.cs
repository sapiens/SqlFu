using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;

namespace SqlFu.Migrations
{
    public class DatabaseMigration:IConfigureMigrationsRunner
    {
        private readonly IAccessDb _db;

        private ILogWriter _log= new ConsoleLogger();
        private List<Assembly> _asm = new List<Assembly>();
        IResolveDependencies _resolver= new DependencyContainerWrapper(t=> Activator.CreateInstance(t));
        public DatabaseMigration(IAccessDb db)
        {
            _db = db;
        }

        public static IConfigureMigrationsRunner ConfigureFor(IAccessDb db)
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
            _asm.Add(typeof(T).Assembly);
            return this;
        }

        public IConfigureMigrationsRunner UseLogger(ILogWriter logger)
        {
            logger.MustNotBeNull();
            _log = logger;
            return this;
        }

        public IConfigureMigrationsRunner UseResolver(IResolveDependencies resolver)
        {
            resolver.MustNotBeNull();
            _resolver = resolver;
            return this;
        }

        public IRunMigrations Build()
        {
            var types=_asm
                .SelectMany(a=>AssemblyExtensions.GetTypesImplementing<IMigrateDatabase>(a)
                                   .Select(t=>(IMigrateDatabase)_resolver.Resolve(t)))
                .Where(t=>t.CurrentVersion!=null)
                .ToArray();
            if (types.Length==0)
            {
                throw new MigrationNotFoundException("None of the provided assemblies contained SqlFu migrations");
            }
            return new MigrationsRunner(_db,types,_log);
        }

        public void PerformAutomaticMigrations()
        {
            var migrator = new AutomaticMigration(_db, Build());
            migrator.Execute();
        }
    }
}