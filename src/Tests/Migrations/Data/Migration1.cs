using System.Text;
using SqlFu;
using SqlFu.Migrations;

namespace Tests.Migrations.Data
{
    [Migration("1.0.0", "1.0.1", SchemaName = "TestSchema")]
    public class Migration1:AbstractMigrationTask
    {
        private readonly StringBuilder _builder=new StringBuilder();

        public Migration1()
        {
           
        }
        public Migration1(StringBuilder builder)
        {
            _builder = builder;
        }

        public override void Execute(IAccessDb db)
        {
            _builder.Append("1");
        }
    }
}