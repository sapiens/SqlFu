using System;
using System.Reflection;
using CavemanTools.Infrastructure;
using SqlFu.Migrations.Automatic;

namespace SqlFu.Migrations
{
    public interface IConfigureMigrationsRunner
    {
        IConfigureMigrationsRunner SearchMigrations(params Assembly[] asm);
        IConfigureMigrationsRunner SearchAssemblyOf<T>();
      
        IConfigureMigrationsRunner WithResolver(IResolveDependencies resolver);
        IConfigureMigrationsRunner WithResolver(Func<Type,object> resolver);
        IManageMigrations Build();

        IAutomaticMigration BuildAutomaticMigrator();

        /// <summary>
        /// Tries to install/update all the specified schemas.
        /// If no schema is specified it tries to process all schemas found
        /// </summary>
        /// <param name="schemas"></param>
        void PerformAutomaticMigrations(params string[] schemas);

        /// <summary>
        /// Uninstalls the specified schemas or all of them
        /// </summary>
        /// <param name="schemas"></param>
        /// <returns></returns>
        void Uninstall(params string[] schemas);
    }
}