namespace SqlFu.Migrations
{
    public interface IRunMigrations
    {
        void Run(params IMigrateDatabase[] tasks);
        void Migrate(string currentVersion, string nextVersion);
        void MigrateToLatestFrom(string currentVersion);
        string LatestVersionAvailable { get; }
        void InstallSchema();                                              
    }
}