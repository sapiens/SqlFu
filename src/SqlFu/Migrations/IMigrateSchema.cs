namespace SqlFu.Migrations
{
    public interface IMigrateSchema
    {
        /// <summary>
        /// Gets or sets the runner used to execute tasks
        /// </summary>
        IRunMigrations Runner { get; set; }

        string SchemaName { get; }
        void Migrate(string currentVersion, string nextVersion);
        void MigrateToLatestFrom(string currentVersion);
        string LatestVersionAvailable { get; }
        int Priority { get; set; }
        void InstallSchema();
    }
}