using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using SqlFu.Migrations.Automatic.Models;

namespace SqlFu.Migrations.Automatic
{
    internal class AutomaticMigration : IAutomaticMigration
    {
        private readonly DbConnection _db;
        private readonly IManageMigrations _migrations;
        private readonly IEnumerable<IUninstallSchema> _unistallers;
        private readonly IRunMigrations _runner;
        internal const string TableName = "_SqlFuMigrationTracker";
        internal const string SchemaName = "_AutomaticMigration";

        public AutomaticMigration(DbConnection db, IManageMigrations migrations,IEnumerable<IUninstallSchema> unistallers)
        {
            _db = db;
            _migrations = migrations;
            _unistallers = unistallers;
            _runner = new MigrationTaskRunner(db);
           
        }

        public void Install(params string[] schemas)
        {
            UpdateSelf();
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



        public void Uninstall(params string[] schemas)
        {
            if (schemas.Length == 0) return;
            UpdateSelf();
            
            using (var t = _migrations.StartUnitOfWork())
            {
                
                var tasks = _unistallers.Where(s => schemas.Any(d => d == s.SchemaName && d!=SchemaName));

                foreach (var task in tasks)
                {
                    task.Uninstall(_db);
                }
                _db.DeleteFrom<SqlFuMigrationTracker>(d => schemas.Contains(d.SchemaName));
                t.Commit();
            }
                              
        }

        public void SelfDestroy()
        {
            _db.DropTable<SqlFuMigrationTracker>();
        }

        private void AppendVersion(string schema, string version)
        {
            _db.Insert(new SqlFuMigrationTracker()
                {
                    SchemaName = schema,
                    Version = version,
                    TimeOfUpdate = DateTime.UtcNow
                });
        }

        private void UpdateSelf()
        {
            var migrator = new SchemaMigrationExecutor(_runner, GetMigratorTasks(), SchemaName);

            using (var t = _db.BeginTransaction())
            {
                if (!_db.TableExists<SqlFuMigrationTracker>())
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
            return
                _db.From<SqlFuMigrationTracker>()
                    .Select(d => d.Version)
                    .Where(d => d.SchemaName == schema)
                    .OrderByDescending<SqlFuMigrationTracker>(d => d.Id)
                    .Limit(1)
                    .GetValue();
            //return _db.GetValue<string>("select " + _db.EscapeIdentifier("Version") + " from " + _db.EscapeIdentifier(TableName) + " where {0}=@0 order by {1} desc".ToFormat(_db.EscapeIdentifier("SchemaName"),_db.EscapeIdentifier("Id")),
            //                            schema);
        }
    }
}