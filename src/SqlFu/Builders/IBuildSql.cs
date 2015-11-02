namespace SqlFu.Builders
{
    public interface IGenerateSql<T> : IGenerateSql
    {
        IGenerateSql<TProjection> MapTo<TProjection>();
    }
}