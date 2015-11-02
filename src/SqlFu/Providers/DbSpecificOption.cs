namespace SqlFu.Providers
{
    public class DbSpecificOption
    {
        public DbSpecificOption(string providerId, string name)
        {
            ProviderId = providerId;
            Name = name;
        }

        public object Value { get; set; }

        public string ProviderId { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return "";
        }
    }
}