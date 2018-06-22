using System;

namespace SqlFu
{
    public interface IInsertedId
    {
        T GetInsertedId<T>();
    }

    public class InsertedId : IInsertedId
    {
        private readonly object _value;
        public static InsertedId OkWithNoResult=new InsertedId();
        public static InsertedId NotOkWithNoResult=new InsertedId(false);

        private InsertedId(bool success=true)
        {
            InsertWasSuccessful = success;
        }
        public InsertedId(object value)
        {
            _value = value;
            IsEmpty = value == null || value == DBNull.Value;
        }

        public bool InsertWasSuccessful { get; private set; } = true;

        public bool IsEmpty { get; private set; } = true;

        public T GetInsertedId<T>()
        {
            if(IsEmpty) throw  new InvalidOperationException("There was nothing returned by the insert. Please check the 'IsEmpty' property first.");
            if (_value!=null && _value is long)
            {
                var value = (int)(long)_value;
                return (T) (object)value;//ugly hack for providers returning longs
            }
            return (T) _value;
        }
    }
}