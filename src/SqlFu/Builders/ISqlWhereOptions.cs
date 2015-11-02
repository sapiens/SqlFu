namespace SqlFu.Builders
{
    public interface ISqlWhereOptions<T, TProjection>
    {
        ISqlWhereConnectors<T, TProjection> In(IBuildSql subquery);
        ISqlWhereConnectors<T, TProjection> NotIn(IBuildSql subquery);

    }
}