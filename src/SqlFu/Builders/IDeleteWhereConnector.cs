namespace SqlFu.Builders
{
    public interface IDeleteWhereConnector<T>:IDeleteWhere<T>
    {
        IDeleteWhere<T> And { get; }
        IDeleteWhere<T> Or { get; }
    }
}