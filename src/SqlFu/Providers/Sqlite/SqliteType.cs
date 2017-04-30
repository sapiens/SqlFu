using System;

namespace SqlFu.Providers.Sqlite
{
    public class SqliteType:DbTypes
    {
        public const string Integer = "Integer";
        public const string Real = "Real";
        public const string Text = "Text";
        public const string Blob = "Blob";
        public const string Numeric = "Numeric";

        public SqliteType()
        {
            this[typeof(string)] = Text;
            this[typeof(int)] = Integer;
            this[typeof(bool)] = Integer;
            this[typeof(int?)] = Integer;
            this[typeof(bool?)] = Integer;
            this[typeof(Guid)] = Text;   
            this[typeof(Guid?)] = Text;
            this[typeof(byte[])] = Blob;
            this[typeof(DateTimeOffset)] = Text;
            this[typeof(DateTimeOffset?)] = Text;
            this[typeof(TimeSpan)] = Text;
            this[typeof(TimeSpan?)] = Text;
            this[typeof(long)] = Integer;

        }
    }
}