namespace SqlFu.Builders
{
    public interface IDeleteFromSubQuery<T>:IBuildSql
    {
        IDeleteWhereConnector<T> In(IBuildSql subquery);
        IDeleteWhereConnector<T> NotIn(IBuildSql subquery);
    }
}