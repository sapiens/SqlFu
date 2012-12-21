using System.Reflection;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;
using SqlFu.Migrations.Automatic;

namespace SqlFu.Migrations
{
    public interface IConfigureMigrationsRunner
    {
        IConfigureMigrationsRunner SearchAssembly(params Assembly[] asm);
        IConfigureMigrationsRunner SearchAssemblyOf<T>();
        IConfigureMigrationsRunner WithLogger(ILogWriter logger);
        IConfigureMigrationsRunner WithResolver(IResolveDependencies resolver);
        IManageMigrations Build();

        IAutomaticMigration BuildAutomaticMigrator();

        /// <summary>
        /// Tries to install/update all the specified schemas.
        /// If no schema is specified it tries to process all schemas found
        /// </summary>
        /// <param name="schemas"></param>
        void PerformAutomaticMigrations(params string[] schemas);
    }
}