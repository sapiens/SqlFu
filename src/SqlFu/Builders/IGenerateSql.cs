namespace SqlFu.Builders
{
    public interface IGenerateSql
    {
       CommandConfiguration GetCommandConfiguration();   
    }

    public interface IGenerateSql<T> : IGenerateSql
    {
        IGenerateSql<TProjection> MapTo<TProjection>();
    }
}