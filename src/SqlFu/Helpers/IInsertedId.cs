namespace SqlFu
{
    public interface IInsertedId
    {
        T GetInsertedId<T>();
    }

    public class InsertedId : IInsertedId
    {
        private readonly object _value;

        public InsertedId(object value)
        {
            _value = value;
        }

        public T GetInsertedId<T>()
        {
            return (T) _value;
        }
    }
}