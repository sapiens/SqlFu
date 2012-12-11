namespace SqlFu.DDL
{
    public enum ForeignKeyRelationCascade
    {
        NoAction,
        Cascade,
        SetNull,
        SetDefault,
        Restrict
    }
}