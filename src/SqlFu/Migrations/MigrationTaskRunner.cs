using System.Collections.Generic;
using System.Data.Common;
using CavemanTools;
using CavemanTools.Logging;

namespace SqlFu.Migrations
{
    public class MigrationTaskRunner : IRunMigrations
    {
        private readonly DbConnection _db;
        

        public MigrationTaskRunner(DbConnection db)
        {
            _db = db;
            
        }

        public void Run(params IMigrationTask[] tasks)
        {
            if (tasks.IsNullOrEmpty()) return;
            using (var t = _db.BeginTransaction())
            {
                foreach (var task in tasks)
                {
                    if (task.NextVersion == null)
                    {
                        this.LogInfo("Installing database schema '{1}' with version {0}", task.CurrentVersion,
                                     task.SchemaName);
                    }
                    else
                    {
                        this.LogInfo("Executing '{2}' migration from version {0} to version {1}", task.CurrentVersion,
                                     task.NextVersion, task.SchemaName);
                    }
               task.Execute(_db);
                }
                t.Commit();
            }
        }

        public IUnitOfWork StartUnitOfWork()
        {
            return new DatabaseUnitOfWork(_db.BeginTransaction());
        }

        public void Uninstall(params IUninstallSchema[] tasks)
        {
            if (tasks.IsNullOrEmpty()) return;
            using (var t = _db.BeginTransaction())
            {
                foreach (var task in tasks)
                {
                    this.LogInfo("Uninstalling '{0}'", task.SchemaName);
                    
                    task.Uninstall(_db);
                }
                t.Commit();
            }
        }
    }
}