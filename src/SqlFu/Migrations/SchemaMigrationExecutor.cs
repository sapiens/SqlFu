using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools;

namespace SqlFu.Migrations
{
    public class SchemaMigrationExecutor : IMigrateSchema
    {
        private IRunMigrations _runner;
        private readonly IEnumerable<IMigrationTask> _tasks;
        private readonly string _latest;


        public SchemaMigrationExecutor(IRunMigrations runner, IEnumerable<IMigrationTask> tasks, string schemaName)
        {
            runner.MustNotBeNull();
            tasks.MustNotBeNull();
            schemaName.MustNotBeEmpty();

            if (tasks.Any(t => t.SchemaName != schemaName))
                throw new ArgumentException("Some of the provided tasks don't belong to the specified schema");
            _runner = runner;
            _tasks = tasks;
            SchemaName = schemaName;

            var late = _tasks.FirstOrDefault(t => t.CurrentVersion != null && t.NextVersion == null);
            if (late != null)
            {
                _latest = late.CurrentVersion.ToString();
                Priority = late.Priority;
            }
            var lateUpgrade = _tasks.Where(d => d.NextVersion != null).Max(d => d.NextVersion);
            if (lateUpgrade != null)
            {
                _latest = lateUpgrade.CompareTo(late.CurrentVersion) > 0
                              ? lateUpgrade.ToString()
                              : late.CurrentVersion.ToString();
            }
        }

        internal IEnumerable<IMigrationTask> Tasks
        {
            get { return _tasks; }
        }


        /// <summary>
        /// Gets or sets the runner used to execute tasks
        /// </summary>
        public IRunMigrations Runner
        {
            get { return _runner; }
            set
            {
                value.MustNotBeNull();
                _runner = value;
            }
        }

        public string SchemaName { get; set; }

        public void Migrate(string currentVersion, string nextVersion)
        {
            var now = new SemanticVersion(currentVersion);
            var next = new SemanticVersion(nextVersion);
            _runner.Run(
                _tasks.Where(
                    t =>
                    t.CurrentVersion.CompareTo(now) >= 0 &&
                    (t.NextVersion != null && t.NextVersion.CompareTo(next) <= 0)).OrderBy(t => t.CurrentVersion).
                       ToArray());
        }

        public void MigrateToLatestFrom(string currentVersion)
        {
            var vers = new SemanticVersion(currentVersion);
            _runner.Run(
                _tasks.Where(t => t.CurrentVersion.CompareTo(vers) >= 0 && (t.NextVersion != null))
                      .OrderBy(t => t.CurrentVersion)
                      .ToArray());
        }

        public string LatestVersionAvailable
        {
            get { return _latest; }
        }

        public int Priority { get; set; }

        public void InstallSchema()
        {
            var tasks = _tasks.Where(t => t.CurrentVersion.ToString() == _latest);
            if (tasks.Count() > 1)
                throw new InvalidOperationException(
                    string.Format(
                        "Found more than one migration task used to setup the database schema '{0}'. Each schema must have only one such task",
                        SchemaName));
            var task = tasks.FirstOrDefault();
            if (task == null)
            {
                throw new MigrationNotFoundException("There is no migration that can be used to setup the database");
            }
            _runner.Run(task);
        }
    }
}