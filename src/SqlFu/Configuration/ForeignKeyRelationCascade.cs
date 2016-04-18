namespace SqlFu.Configuration
{
    public enum ForeignKeyRelationCascade
    {
        NotSet,
        NoAction,
        Cascade,
        SetNull,
        SetDefault,
        Restrict
    }
}