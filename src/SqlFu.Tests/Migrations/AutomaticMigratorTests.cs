using System.Threading.Tasks;
using SqlFu.Migrations;
using SqlFu.Migrations.Automatic.Models;
using Xunit;

namespace SqlFu.Tests.Migrations
{
    public class AutomaticMigratorTests
    {
        public AutomaticMigratorTests()
        {
         
        
        }

        [Fact]
        public async Task self_install()
        {
            using (var d = Setup.GetConnection())
            {
                var migrator = DatabaseMigration.ConfigureFor(d)
                    .SearchAssemblyOf<SqlFuMigrationTracker>()
                    .BuildAutomaticMigrator();
                migrator.Install();
                migrator.SelfDestroy();
            }
        }

    }
}