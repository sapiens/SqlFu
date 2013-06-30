using System.Data.Common;
using System.Reflection;
using CavemanTools;

namespace SqlFu.Migrations
{
    public abstract class AbstractMigrationTask : IMigrationTask
    {
        public AbstractMigrationTask()
        {
            var attr = GetType().GetSingleAttribute<MigrationAttribute>();
            if (attr != null)
            {
                CurrentVersion = attr.From;
                NextVersion = attr.To;
                SchemaName = attr.SchemaName;
                Priority = attr.Priority;
            }
        }

        public SemanticVersion CurrentVersion { get; private set; }
        public SemanticVersion NextVersion { get; private set; }
        public string SchemaName { get; private set; }
        public abstract void Execute(DbConnection db);
        public int Priority { get; private set; }
    }
}