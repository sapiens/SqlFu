namespace SqlFu.DDL
{
    public class DbSpecificOption
    {
        public DbSpecificOption(string name, DbEngine engine)
        {
            Name = name;
            Engine = engine;
        }

        public DbEngine Engine { get; private set; }

        public object Value { get; set; }

        public string Name { get; private set; }


        public bool IsSameOption(DbSpecificOption other)
        {
            if (other == null) return false;
            return Engine == other.Engine && Name == other.Name;
        }

        public override string ToString()
        {
            return "";
        }
    }
}