using System.Data;
using SqlFu.DDL;

namespace SqlFu.Migrations.Automatic
{
    [Migration("1.0.0", SchemaName = AutomaticMigration.SchemaName)]
    public class AutomaticMigrationSetupTask : AbstractMigrationTask
    {
        /// <summary>
        /// Task is executed automatically in a transaction
        /// </summary>
        /// <param name="db"/>
        public override void Execute(IAccessDb db)
        {
            var tbl = db.DatabaseTools.GetCreateTableBuilder(AutomaticMigration.TableName, IfTableExists.Ignore);
            tbl.Columns
               .Add("Id", DbType.Int32, isNullable: false, autoIncrement: true).AsPrimaryKey()
               .Add("SchemaName", DbType.String, "50")
               .Add("Version", DbType.AnsiString, size: "25", isNullable: false)
               .Add("TimeOfUpdate", DbType.DateTime, isNullable: false);
            tbl.ExecuteDDL();
        }
    }
}