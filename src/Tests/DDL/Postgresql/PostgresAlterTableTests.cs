using SqlFu;
using SqlFu.DDL.Generators.Postgresql;
using Xunit;

namespace Tests.DDL.Postgresql
{
    public class PostgresAlterTableTests:CommonAlterTableTests
    {
        public PostgresAlterTableTests()
        {
            
        }
        protected override DbEngine Engine
        {
            get { return DbEngine.PostgreSQL;}
        }

        [Fact]
        public void drop_column_options()
        {
            Table.Columns.Drop("ParentId")
                 .WithOptionsForDatabase(DbEngine.PostgreSQL, PostgresOptions.Drop.Column(DropOptionType.Cascade));
            Table.ExecuteDDL();
            Assert.False(Db.DatabaseTools.TableHasColumn(TableName,"ParentId"));
        }
    }
}