using System;
using System.Reflection;

namespace SqlFu.Migrations
{
    public static class Extensions
    {
         public static IConfigureMigrationsRunner SearchCurrentAssembly(this IConfigureMigrationsRunner config)
         {
             config.MustNotBeNull();
             return config.SearchAssembly(Assembly.GetCallingAssembly());
         }
    }
}