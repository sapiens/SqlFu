using System;

namespace SqlFu.DDL.Generators.MySql
{
    public static class MySqlOptions
    {
        static MySqlOptions()
        {
            Index = new MySqlIndexOptions();
            Table = new MySqlTableOptions();
        }

        public static MySqlIndexOptions Index { get; private set; }
        public static MySqlTableOptions Table { get; private set; }
    }

    public class MySqlTableOptions
    {
        internal const string Engine = "ENGINE";
        internal const string AUTO_INCREMENT = "AUTO_INCREMENT";
        internal const string Character_Set = "CHARACTER SET";
        internal const string COLLATE = "COLLATE";

        public DbSpecificOption EngineIs(TableEngineType type)
        {
            return new KeyValueOption(Engine, type, DbEngine.MySql) {Symbol = "="};
        }

        public DbSpecificOption AutoIncrementValueIs(object value)
        {
            return new KeyValueOption(AUTO_INCREMENT, value, DbEngine.MySql) {Symbol = "="};
        }

        public DbSpecificOption CharacterSet(string set)
        {
            set.MustNotBeEmpty();
            return new KeyValueOption(Character_Set, set, DbEngine.MySql) {Symbol = "="};
        }

        public DbSpecificOption Collate(string value)
        {
            value.MustNotBeEmpty();
            return new KeyValueOption(COLLATE, value, DbEngine.MySql) {Symbol = "="};
        }
    }

    public enum TableEngineType
    {
        MyIsam,
        InnoDb,
        Memory,
        Archive,
        Csv
    }

    public class MySqlIndexOptions
    {
        public DbSpecificOption IsType(MySqlIndexType type)
        {
            return new KeyValueOption(Using, type, DbEngine.MySql);
        }

        internal const string Using = "USING";
    }

    public enum MySqlIndexType
    {
        BTREE,
        HASH
    }
}