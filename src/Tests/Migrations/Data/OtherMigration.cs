using SqlFu;
using SqlFu.Migrations;

namespace Tests.Migrations.Data
{
    [Migration("1.0.1", SchemaName = "Other",Priority = 100)]
    public class OtherMigration:AbstractMigrationTask
    {
        /// <summary>
        /// Task is executed automatically in a transaction
        /// </summary>
        /// <param name="db"/>
        public override void Execute(IAccessDb db)
        {
            
        }
    }
}