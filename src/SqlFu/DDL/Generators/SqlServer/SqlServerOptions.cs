using SqlFu.DDL.Generators.SqlServer.Options;

namespace SqlFu.DDL.Generators.SqlServer
{
    public static class SqlServerOptions
    {
        static SqlServerOptions()
        {
            ColumnIs = new SqlServerColumnOptions();
            PrimaryKeyIs = new SqlServerPrimaryKeyOptions();
            Index = new SqlServerIndexOptions();
        }

        public static SqlServerColumnOptions ColumnIs { get; private set; }
        public static SqlServerPrimaryKeyOptions PrimaryKeyIs { get; private set; }
        public static SqlServerIndexOptions Index { get; private set; }

        internal const string Sparse = "SPARSE";
        internal const string RowGuidCol = "ROWGUIDCOL";
        internal const string Include = "INCLUDE";
        internal const string Ignore_Dup_Key = "IGNORE_DUP_KEY";
        internal const string Drop_Existing = "DROP_EXISTING";
        internal const string Clustered = "CLUSTERED";
        internal const string NonClustered = "NONCLUSTERED";
    }

    public class SqlServerColumnOptions
    {
        public DbSpecificOption RowGuidCol()
        {
            return new NameOnlyOption(SqlServerOptions.RowGuidCol, DbEngine.SqlServer);
        }

        public DbSpecificOption Sparse()
        {
            return new NameOnlyOption(SqlServerOptions.Sparse, DbEngine.SqlServer);
        }
    }

    public class SqlServerIndexOptions : SqlServerPrimaryKeyOptions
    {
        public DbSpecificOption Include(params string[] columns)
        {
            return new IndexIncludeOption(columns);
        }

        public DbSpecificOption IgnoreDupKey(bool on)
        {
            return new KeyValueOption(SqlServerOptions.Ignore_Dup_Key, on ? "ON" : "OFF", DbEngine.SqlServer)
                {
                    Symbol = "="
                };
        }

        public DbSpecificOption DropExisting(bool on)
        {
            return new KeyValueOption(SqlServerOptions.Drop_Existing, on ? "ON" : "OFF", DbEngine.SqlServer)
                {
                    Symbol = "="
                };
        }
    }

    public class SqlServerPrimaryKeyOptions
    {
        public DbSpecificOption Clustered()
        {
            return new NameOnlyOption(SqlServerOptions.Clustered, DbEngine.SqlServer);
        }

        public DbSpecificOption NonClustered()
        {
            return new NameOnlyOption(SqlServerOptions.NonClustered, DbEngine.SqlServer);
        }
    }
}