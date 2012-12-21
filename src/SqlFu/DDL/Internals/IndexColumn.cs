namespace SqlFu.DDL.Internals
{
    internal class IndexColumn
    {
        public IndexColumn(string name)
        {
            Name = name;
            Options = new DbEngineOptions();
        }

        public string Name { get; set; }

        public bool Ascending { get; set; }

        public DbEngineOptions Options { get; private set; }
    }
}