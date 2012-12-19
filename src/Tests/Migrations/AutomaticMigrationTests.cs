using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using SqlFu;
using SqlFu.Migrations;
using SqlFu.Migrations.Automatic;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class AutomaticMigrationTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private DbAccess _db;

        public AutomaticMigrationTests()
        {
            _db = Setup.GetDb();
        }

        //[Fact]
        public void automatic_a_test()
        {            
            DatabaseMigration.ConfigureFor(_db)
                .WithLogger(new ConsoleLogger())
                //.WithResolver(ActivatorContainer.Instance)
                .SearchAssemblyOf<AutomaticMigrationTests>()
                .PerformAutomaticMigrations();
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
           // _db.DatabaseTools.DropTable(AutomaticMigration.TableName);
        }
    }
}