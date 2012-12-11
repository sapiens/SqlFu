using System.Reflection;
using CavemanTools;

namespace SqlFu.Migrations
{
    public abstract class AbstractMigrationTask:IMigrateDatabase
    {
        public AbstractMigrationTask()
        {
            var attr = GetType().GetSingleAttribute<MigrationAttribute>();
            if (attr!=null)
            {
                CurrentVersion = attr.From;
                NextVersion = attr.To;
            }
        }
        public SemanticVersion CurrentVersion { get; private set; }
        public SemanticVersion NextVersion { get; private set; }
        public abstract void Execute(IAccessDb db);    
    }
}