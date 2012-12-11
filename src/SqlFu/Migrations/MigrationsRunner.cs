using System.Collections.Generic;
using System.Linq;
using CavemanTools;
using CavemanTools.Logging;

namespace SqlFu.Migrations
{
    public class MigrationsRunner:IRunMigrations
    {
        private readonly IAccessDb _db;
        private readonly IEnumerable<IMigrateDatabase> _tasks;
        private readonly ILogWriter _logger;
        private string _latest;

        public MigrationsRunner(IAccessDb db,IEnumerable<IMigrateDatabase> tasks,ILogWriter logger)
        {
            _db = db;
            _tasks = tasks;
            _logger = logger;

            var late = _tasks.FirstOrDefault(t => t.NextVersion == null);
            if (late!=null)
            {
                _latest = late.CurrentVersion.ToString();
            }
            var lateUpgrade = _tasks.Where(d => d.NextVersion != null).Max(d => d.NextVersion);
            if (lateUpgrade!=null)
            {
                _latest = lateUpgrade.CompareTo(late.CurrentVersion) > 0
                              ? lateUpgrade.ToString()
                              : late.CurrentVersion.ToString();
            }
        }

        internal IEnumerable<IMigrateDatabase> Tasks
        {
            get { return _tasks; }
        }

        public void Run(params IMigrateDatabase[] tasks)
        {
           using(var t=_db.BeginTransaction())
           {
               foreach (var task in tasks)
               {
                   if (task.NextVersion == null)
                   {
                       _logger.Info("Installing database schema with version {0}", task.CurrentVersion);
                   }
                   else
                   {
                       _logger.Info("Executing migration from version {0} to version {1}", task.CurrentVersion, task.NextVersion);
                   }
                   task.Execute(_db);
               }   
               t.Commit();
           }
            
        }

        public void Migrate(string currentVersion, string nextVersion)
        {
            var now = new SemanticVersion(currentVersion);
            var next = new SemanticVersion(nextVersion);
            Run(
                _tasks.Where(
                    t =>
                    t.CurrentVersion.CompareTo(now) >= 0 &&
                    (t.NextVersion != null && t.NextVersion.CompareTo(next) <= 0)).OrderBy(t => t.CurrentVersion).
                    ToArray());
        }

        public void MigrateToLatestFrom(string currentVersion)
        {
            var vers = new SemanticVersion(currentVersion);
            Run(_tasks.Where(t=>t.CurrentVersion.CompareTo(vers)>=0 && (t.NextVersion!=null)).OrderBy(t=>t.CurrentVersion).ToArray());            
        }

        public string LatestVersionAvailable
        {
            get { return _latest; }
        }

        public void InstallSchema()
        {
            var task = _tasks.FirstOrDefault(t => t.CurrentVersion != null && t.NextVersion == null);
            if (task==null)
            {
                throw new MigrationNotFoundException("There is no migration that can be used to setup the database");
            }
            Run(task);
        }
    }
}