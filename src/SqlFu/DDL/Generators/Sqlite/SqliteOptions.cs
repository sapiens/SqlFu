namespace SqlFu.DDL.Generators.Sqlite
{
    public class SqliteOptions
    {
        static SqliteOptions()
        {
            Constraint = new SqliteConstraint();
        }

        public static SqliteConstraint Constraint { get; private set; }
        internal const string On_Conflict = "on conflict";
    }

    public class SqliteConstraint
    {
        public KeyValueOption OnConflict(SqliteConflictClause clause)
        {
            return new KeyValueOption(SqliteOptions.On_Conflict, clause, DbEngine.SQLite);
        }
    }

    internal class OnConflictOption : DbSpecificOption
    {
        public const string Id = "on conflict";

        public OnConflictOption(SqliteConflictClause clause) : base(Id, DbEngine.SQLite)
        {
            Value = clause.ToString();
        }

        public override string ToString()
        {
            return "on conflict " + Value;
        }
    }

    public enum SqliteConflictClause
    {
        Rollback,
        Abort,
        Fail,
        Ignore,
        Replace
    }
}