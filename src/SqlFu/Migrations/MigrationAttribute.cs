using System;
using CavemanTools;

namespace SqlFu.Migrations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MigrationAttribute : Attribute
    {
        private string _schemaName = DatabaseMigration.DefaultSchemaName;

        public MigrationAttribute(string fromVersion, string toVersion)
        {
            From = fromVersion;
            To = toVersion;
        }

        public SemanticVersion From { get; set; }
        public SemanticVersion To { get; set; }

        /// <summary>
        /// Optional schema name
        /// </summary>
        public string SchemaName
        {
            get { return _schemaName; }
            set
            {
                value.MustNotBeEmpty();
                _schemaName = value;
            }
        }

        /// <summary>
        /// Executed from highest to lowest
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Installs schema for the specified version
        /// </summary>
        /// <param name="currentVersion"></param>
        public MigrationAttribute(string currentVersion)
        {
            From = currentVersion;
        }
    }
}