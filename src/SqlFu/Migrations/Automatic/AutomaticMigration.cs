using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using CavemanTools.Logging;

namespace SqlFu.Migrations.Automatic
{
    internal class AutomaticMigration : IAutomaticMigration
    {
        private readonly DbConnection _db;
        private readonly IManageMigrations _migrations;
        private readonly IRunMigrations _runner;
        internal const string TableName = "SqlFuMigrationTracker";
        internal const string SchemaName = "AutomaticMigration";

        public AutomaticMigration(DbConnection db, IManageMigrations migrations, ILogWriter logger)
        {
            _db = db;
            _migrations = migrations;
            _runner = new MigrationTaskRunner(db, logger);
            UpdateSelf();
        }

        public void Execute(params string[] schemas)
        {
            IEnumerable<IMigrateSchema> tasks = _migrations.Schemas;
            if (schemas.Length > 0)
            {
                tasks = tasks.Where(s => schemas.Any(d => d == s.SchemaName));
            }
            using (var t = _migrations.StartUnitOfWork())
            {
                foreach (var schema in tasks)
                {
                    var version = GetInstalledVersion(schema.SchemaName);
                    if (version.IsNullOrEmpty())
                    {
                        schema.InstallSchema();
                        AppendVersion(schema.SchemaName, schema.LatestVersionAvailable);
                    }
                    else
                    {
                        if (version != schema.LatestVersionAvailable)
                        {
                            schema.MigrateToLatestFrom(version);
                            AppendVersion(schema.SchemaName, schema.LatestVersionAvailable);
                        }
                    }
                }
                t.Commit();
            }
        }

        public void Untrack(params string[] schemas)
        {
            if (schemas.Length == 0) return;
            _db.ExecuteCommand("delete from " + _db.EscapeIdentifier(TableName) + " where {0} in (@0)".ToFormat(_db.EscapeIdentifier("SchemaName")), schemas.ToList());
        }

        private void AppendVersion(string schema, string version)
        {
            _db.Insert(new MigrationTrack
                {
                    SchemaName = schema,
                    Version = version
                });
        }

        private void UpdateSelf()
        {
            var migrator = new SchemaMigrationExecutor(_runner, GetMigratorTasks(), SchemaName);

            using (var t = _db.BeginTransaction())
            {
                if (!_db.DatabaseTools().TableExists(TableName))
                {
                    migrator.InstallSchema();
                    AppendVersion(SchemaName, migrator.LatestVersionAvailable);
                }
                else
                {
                    var latest = GetInstalledVersion(SchemaName);
                    if (latest.IsNullOrEmpty())
                    {
                        migrator.InstallSchema();
                        AppendVersion(SchemaName, migrator.LatestVersionAvailable);
                    }
                    else
                    {
                        if (latest != migrator.LatestVersionAvailable)
                        {
                            migrator.MigrateToLatestFrom(latest);
                            AppendVersion(SchemaName, migrator.LatestVersionAvailable);
                        }
                    }
                }

                t.Commit();
            }
        }

        private IEnumerable<IMigrationTask> GetMigratorTasks()
        {
            var tasks = GetType().Assembly.GetTypesImplementing<IMigrationTask>(true)
                                 .Select(t => Activator.CreateInstance(t))
                                 .Cast<IMigrationTask>();
            return tasks.Where(t => t.SchemaName == SchemaName);
        }


        private string GetInstalledVersion(string schema)
        {
            return _db.GetValue<string>("select " + _db.EscapeIdentifier("Version") + " from " + _db.EscapeIdentifier(TableName) + " where {0}=@0 order by {1} desc".ToFormat(_db.EscapeIdentifier("SchemaName"),_db.EscapeIdentifier("Id")),
                                        schema);
        }
    }
}