using System.Collections.Generic;

namespace SqlFu.Migrations
{
    public interface IManageMigrations : IRunMigrations
    {
        IMigrateSchema GetSchemaMigrator(string schemaName = DatabaseMigration.DefaultSchemaName);
        void InstallAllSchemas();
        void Add(IMigrateSchema schema);
        IEnumerable<IMigrateSchema> Schemas { get; }
    }
}