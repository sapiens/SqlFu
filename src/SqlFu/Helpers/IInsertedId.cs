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
            if (_value!=null && _value is long)
            {
                var value = (int)(long)_value;
                return (T) (object)value;//ugly hack for providers returning longs
            }
            return (T) _value;
        }
    }
}