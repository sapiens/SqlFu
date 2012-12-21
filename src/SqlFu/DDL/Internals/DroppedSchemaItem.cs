namespace SqlFu.DDL.Internals
{
    internal class DroppedSchemaItem
    {
        private readonly string _tableName;

        public DroppedSchemaItem(string name, string tableName)
        {
            _tableName = tableName;
            Name = name;
            Options = new DbEngineOptions();
        }

        public bool IsPrimaryKey { get; set; }
        public string Name { get; set; }
        public DbEngineOptions Options { get; private set; }

        public string TableName
        {
            get { return _tableName; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}