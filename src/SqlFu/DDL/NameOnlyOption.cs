namespace SqlFu.DDL
{
    public class NameOnlyOption : DbSpecificOption
    {
        public NameOnlyOption(string name, DbEngine engine) : base(name, engine)
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}