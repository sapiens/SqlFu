using System.Data.Common;
using CavemanTools;

namespace SqlFu.Migrations
{
    public interface IMigrationTask
    {
        /// <summary>
        /// Gets semantic version to upgrade from
        /// </summary>
        SemanticVersion CurrentVersion { get; }

        /// <summary>
        /// Gets semantic version to upgrade to
        /// </summary>
        SemanticVersion NextVersion { get; }

        string SchemaName { get; }

        /// <summary>
        /// Task is executed automatically in a transaction
        /// </summary>
        /// <param name="db"></param>
        void Execute(DbConnection db);

        int Priority { get; }
    }
}