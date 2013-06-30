using System.Data.Common;
using System.Text;
using SqlFu;
using SqlFu.Migrations;

namespace Tests.Migrations.Data
{
    [Migration("1.0.2",SchemaName = "TestSchema",Priority = 3)]
    public class InstallerTask:AbstractMigrationTask
    {
        private readonly StringBuilder _sb=new StringBuilder();

        public InstallerTask()
        {
            
        }
        public InstallerTask(StringBuilder sb)
        {
            _sb = sb;
        }

        public override void Execute(DbConnection db)
        {
            _sb.Append("1.0.2");
        }
    }
}