namespace SqlFu.Builders
{
    public interface IPaged<T>:ISelect<T>
    {
        ISelect<T> Limit(int take, long offset=0);
    }
}