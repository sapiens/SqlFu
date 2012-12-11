using SqlFu;
using SqlFu.Migrations;
using Xunit;
using System;
using System.Diagnostics;

namespace Tests.Migrations
{
    public class AutomaticMigrationTests
    {
        private Stopwatch _t = new Stopwatch();

        public AutomaticMigrationTests()
        {

        }

        //[Fact]
        public void not_a_test()
        {
            DatabaseMigration.ConfigureFor(Setup.GetDb(engine:DbEngine.MySql))
                .SearchAssemblyOf<AutomaticMigrationTests>()
                .PerformAutomaticMigrations();
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }
    }
}