using System;
using CavemanTools;

namespace SqlFu.Migrations
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false)]
    public class MigrationAttribute:Attribute
    {
        public MigrationAttribute(string fromVersion,string toVersion)
        {
            From = fromVersion;
            To = toVersion;
        }

        public SemanticVersion From { get; set; }
        public SemanticVersion To { get; set; }

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