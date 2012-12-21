namespace SqlFu.DDL.Generators.Postgresql
{
    public static class PostgresOptions
    {
        static PostgresOptions()
        {
            Drop = new DropOptions();
        }

        public static DropOptions Drop { get; private set; }
    }

    public enum DropOptionType
    {
        Restrict,
        Cascade
    }

    public class DropOptions
    {
        internal const string Restrict = "restrict";
        internal const string Cascade = "cascade";

        public DbSpecificOption Column(DropOptionType option)
        {
            return new NameOnlyOption(option.ToString(), DbEngine.PostgreSQL);
        }

        public DbSpecificOption Constraint(DropOptionType option)
        {
            return new NameOnlyOption(option.ToString(), DbEngine.PostgreSQL);
        }

        public DbSpecificOption Index(DropOptionType option)
        {
            return new NameOnlyOption(option.ToString(), DbEngine.PostgreSQL);
        }
    }
}