using System;
using SqlFu.Configuration;

namespace SqlFu.Migrations.Automatic.Models
{
    public class SqlFuMigrationTracker
    {
        public int Id { get; set; }
        public string SchemaName { get; set; }
        public string Version { get; set; }
        public DateTime TimeOfUpdate { get; set; }
    }

    public class TrackerMap : APocoToTableMapping<SqlFuMigrationTracker>
    {
        public TrackerMap()
        {
            TableNameIs("_SqlFuMigrationTracker");
            Column(d => d.Id).IsIdentity();
            Column(d => d.SchemaName).HasSize(50).IsNull();
            Column(d => d.Version).HasSize(25);
            
            PrimaryKeyOn(d => d.Id);
            IfTableExists().DropIt();
        }
    }
}