using SqlFu.Configuration;

namespace SqlFu.Builders.CreateTable
{
    public class ForeignKeyDefinition
    {
        public string Name { get; set; }
        public string[] Columns { get; set; }
        public TableName ParentTable { get; set; }
        public string[] ParentColumns { get; set; }

        public ForeignKeyRelationCascade OnUpdate { get; set; }
        public ForeignKeyRelationCascade OnDelete { get; set; }
    }
}