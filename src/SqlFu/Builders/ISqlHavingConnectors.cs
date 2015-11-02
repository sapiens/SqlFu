namespace SqlFu.Builders
{
    public interface ISqlHavingConnectors<T, TProjection> : ISqlHaving<T, TProjection>
    {
        ISqlHaving<T, TProjection> And { get; }
        ISqlHaving<T, TProjection> Or { get; }
    }
}