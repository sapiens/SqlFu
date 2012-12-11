using System.Reflection;
using CavemanTools.Infrastructure;
using CavemanTools.Logging;

namespace SqlFu.Migrations
{
    public interface IConfigureMigrationsRunner
    {
        IConfigureMigrationsRunner SearchAssembly(params Assembly[] asm);
        IConfigureMigrationsRunner SearchAssemblyOf<T>();
        IConfigureMigrationsRunner UseLogger(ILogWriter logger);
        IConfigureMigrationsRunner UseResolver(IResolveDependencies resolver);
        IRunMigrations Build();
        void PerformAutomaticMigrations();
    }
}