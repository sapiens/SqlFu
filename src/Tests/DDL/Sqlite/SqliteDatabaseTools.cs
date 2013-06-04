#if Sqlite
using SqlFu;
using SqlFu.Providers;
using Xunit;


namespace Tests.DDL.Sqlite
{
    public class SqliteDatabaseTools:CommonDatabaseToolsTests
    {
        #region Overrides of ProviderDatabaseToolsTests

        protected override DbEngine Engine
        {
            get { return DbEngine.SQLite; }
        }

        [Fact(Skip = "Not supported")]
        public override void constraint_exists()
        {
            
        }

        protected override void SetupTable()
        {
            Db.ExecuteCommand(@"
create table test
( id int not null,
name varchar(20)
);
create index ix_test on test(name)
");
         
        }

       #endregion
    }
}
#endif