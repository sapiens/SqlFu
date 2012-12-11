using System;
using System.Data;
using SqlFu.DDL;

namespace SqlFu.Migrations
{
    class AutomaticMigration
    {
        private readonly IAccessDb _db;
        private readonly IRunMigrations _runner;
        private IDatabaseTools _tools;
        private const string TableName = "SqlFuMigrationTracker";
        public AutomaticMigration(IAccessDb db,IRunMigrations runner)
        {
            _db = db;
            _runner = runner;
            _tools = db.DatabaseTools;
        }

        public void Execute()
        {
            if (!_tools.TableExists(TableName))
            {
                InstallSchema();
            }
            else
            {
                UpgradeSchema();
            }
        }

        void InstallSchema()
        {
            var tbl=_tools.GetCreateTableBuilder(TableName,IfTableExists.DropIt);
            tbl.Columns
                .Add("Id",DbType.Int32,isNullable:false,autoIncrement:true)
                .Add("Version", DbType.String, size: "25", isNullable: false)
                .Add("Timestamp",DbType.DateTime,isNullable:false);
            tbl.Constraints.AddPrimaryKeyOn("Id");

            using(var t=_db.BeginTransaction())
            {
                tbl.ExecuteDDL();
                _runner.InstallSchema();
                AppendVersion();    
                t.Commit();
            }
            
        }

        void AppendVersion()
        {
            _db.Insert(TableName, new { Version = _runner.LatestVersionAvailable, Timestamp = DateTime.UtcNow });
        }

        void UpgradeSchema()
        {
            var current = _db.GetValue<string>("select Version from " + TableName + " order by Id desc");
            if (current.IsNullOrEmpty())
            {
                InstallSchema();
                return;
            }

            if (current==_runner.LatestVersionAvailable) return;

            using(var t= _db.BeginTransaction())
            {
                _runner.MigrateToLatestFrom(current);
                AppendVersion();
                t.Commit();
            }
        }
    }
}