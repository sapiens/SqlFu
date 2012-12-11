using CavemanTools;

namespace SqlFu.Migrations
{
    public interface IMigrateDatabase
    {
        /// <summary>
        /// Gets semantic version to upgrade from
        /// </summary>
        SemanticVersion CurrentVersion { get; }
        /// <summary>
        /// Gets semantic version to upgrade to
        /// </summary>
        SemanticVersion NextVersion { get; }
        void Execute(IAccessDb db);
    }
}