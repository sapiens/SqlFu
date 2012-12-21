using System;
using System.Collections.Generic;
using System.Linq;
using CavemanTools;

namespace SqlFu.Migrations
{
    public class MigrationsManager : IManageMigrations
    {
        private List<IMigrateSchema> _schemas;
        private readonly IRunMigrations _runner;

        public MigrationsManager(IEnumerable<IMigrateSchema> schemas, IRunMigrations runner)
        {
            schemas.MustNotBeNull();
            schemas.ForEach(s => s.Runner = runner);
            _schemas = Sort(schemas);
            _runner = runner;
        }

        private static List<IMigrateSchema> Sort(IEnumerable<IMigrateSchema> data)
        {
            return data.OrderByDescending(s => s.Priority).ToList();
        }

        public IMigrateSchema GetSchemaMigrator(string schemaName = DatabaseMigration.DefaultSchemaName)
        {
            schemaName.MustNotBeEmpty();
            return _schemas.FirstOrDefault(s => s.SchemaName == schemaName);
        }

        public void Run(params IMigrationTask[] tasks)
        {
            _runner.Run(tasks);
        }

        public IUnitOfWork StartUnitOfWork()
        {
            //runner is shared by the schemas, the transaction applies to all schemas
            return _runner.StartUnitOfWork();
        }

        public void InstallAllSchemas()
        {
            foreach (var schema in _schemas) schema.InstallSchema();
        }

        public void Add(IMigrateSchema schema)
        {
            schema.MustNotBeNull();
            schema.Runner = _runner;
            _schemas.Add(schema);
            _schemas = Sort(_schemas);
        }

        public IEnumerable<IMigrateSchema> Schemas
        {
            get { return _schemas; }
        }
    }
}