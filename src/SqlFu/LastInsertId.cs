using System;

namespace SqlFu
{
    public class LastInsertId
    {
        private readonly object _val;
        public static LastInsertId Empty = new LastInsertId(null);

        public LastInsertId(object o)
        {
            _val = o;
        }

        public bool IsEmpty
        {
            get { return _val == null; }
        }

        public T InsertedId<T>()
        {
            return _val.ConvertTo<T>();
        }
    }
}