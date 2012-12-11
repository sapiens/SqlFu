using System.Collections.Generic;

namespace SqlFu.DDL.Internals
{
    abstract class ConstraintDefinition
    {
        public ConstraintDefinition()
        {
            Options= new DbEngineOptions();
        }
        internal DbEngineOptions Options { get; private set; }
        public string Name { get; set; }
        public string Columns { get; set; }
        public bool Ignore { get; set; }

        Dictionary<DbEngine, string> _redefined = new Dictionary<DbEngine, string>();
        public void Redefine(DbEngine engine, string definition)
        {
            _redefined[engine] = definition;
        }

        public bool IsRedefined(DbEngine engine)
        {
            return _redefined.ContainsKey(engine);
        }

        public string GetDefinition(DbEngine engine)
        {
            return _redefined[engine];
        }
    }

    class UniqueKeyConstraint:ConstraintDefinition
    {
        public bool IsPrimary { get; set; }
    }

    class CheckConstraint:ConstraintDefinition
    {
        public string Expression { get; set; }
    }

    class ForeignKeyConstraint:ConstraintDefinition
    {
        public string ParentTable { get; set; }
        public string ParentColumn { get; set; }
        public ForeignKeyRelationCascade OnUpdate { get; set; }
        public ForeignKeyRelationCascade OnDelete { get; set; }
    }
}