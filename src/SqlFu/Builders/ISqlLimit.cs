namespace SqlFu.Builders
{
    public interface ISqlLimit<TProjection>:IQuerySql<TProjection>
    {
        IQuerySql<TProjection> Limit(int take,long skip=0);
    }
}