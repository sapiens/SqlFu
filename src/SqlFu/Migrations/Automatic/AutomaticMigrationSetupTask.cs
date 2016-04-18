using System.Data.Common;
using SqlFu.Migrations.Automatic.Models;

namespace SqlFu.Migrations.Automatic
{
    [Migration("1.0.0", SchemaName = AutomaticMigration.SchemaName)]
    public class AutomaticMigrationSetupTask : AbstractMigrationTask
    {
        /// <summary>
        /// Task is executed automatically in a transaction
        /// </summary>
        /// <param name="db"/>
        public override void Execute(DbConnection db)
        {
           SqlFuManager.Config.AddTableMapping(new TrackerMap());
            db.CreateTable<SqlFuMigrationTracker>();
       
        }

       
    }
}