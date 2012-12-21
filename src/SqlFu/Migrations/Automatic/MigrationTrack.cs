using System;

namespace SqlFu.Migrations.Automatic
{
    [Table(AutomaticMigration.TableName)]
    internal class MigrationTrack
    {
        public MigrationTrack()
        {
            TimeOfUpdate = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public string SchemaName { get; set; }
        public string Version { get; set; }
        public DateTime TimeOfUpdate { get; set; }
    }
}