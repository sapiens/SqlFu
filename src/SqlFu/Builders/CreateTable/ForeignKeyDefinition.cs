using SqlFu.Configuration;

namespace SqlFu.Builders.CreateTable
{
    public class ForeignKeyDefinition
    {
        public string Name { get; set; }
        public string[] Columns { get; set; }
        public TableName ParentTable { get; set; }=new TableName();
        public string[] ParentColumns { get; set; }

        public ForeignKeyRelationCascade OnUpdate { get; set; }
        public ForeignKeyRelationCascade OnDelete { get; set; }
    }

    public class TableName
    {
        public string Name { get; set; }
        public string Schema { get; set; }

        public TableName()
        {
            
        }

        public TableName(string name,string schema="")
        {
            Name = name;
            Schema = schema;
        }
    }
}