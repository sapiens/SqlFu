using System.Collections.Generic;
using System.Data.Common;
using CavemanTools;
using CavemanTools.Logging;

namespace SqlFu.Migrations
{
    public class MigrationTaskRunner : IRunMigrations
    {
        private readonly DbConnection _db;
        private readonly ILogWriter _logger;

        public MigrationTaskRunner(DbConnection db, ILogWriter logger)
        {
            _db = db;
            _logger = logger;
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
                        _logger.Info("Installing database schema '{1}' with version {0}", task.CurrentVersion,
                                     task.SchemaName);
                    }
                    else
                    {
                        _logger.Info("Executing '{2}' migration from version {0} to version {1}", task.CurrentVersion,
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
    }
}