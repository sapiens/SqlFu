using System.Data.Common;
using System.Text;
using SqlFu;
using SqlFu.Migrations;

namespace Tests.Migrations.Data
{
    [Migration("1.0.1", "1.0.2", SchemaName = "TestSchema")]
    public class Migration2:AbstractMigrationTask
    {
        private readonly StringBuilder _builder=new StringBuilder();

        public Migration2()
        {
            
        }
        public Migration2(StringBuilder builder)
        {
            _builder = builder;
        }

        public override void Execute(DbConnection db)
        {
            _builder.Append("2");
        }
    }
}