using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace SqlFu.Mapping.Internals
{
    public class SqlFuDynamic : DynamicObject,IDictionary<string,object>
    {
        private readonly string[] _columns;
        private object[] _values;


        public SqlFuDynamic(string[] columns)
        {
            _columns = columns;
            _values=new object[_columns.Length];
        }

        public bool ContainsKey(string key) => _columns.Any(d => d == key);
        

        public void Add(string key, object value)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out object value)
        {
           value = null;
            var idx = Array.IndexOf(_columns, key);
            if (idx < 0) return false;
            value = _values[idx];
            return true;
        }

        public object this[string key]
        {
            get
            {
                object rez;
                if (!TryGetValue(key, out rez)) throw new KeyNotFoundException();
                return rez;
            }
            set { throw new NotImplementedException(); }
        }

        public ICollection<string> Keys => _columns;
        ICollection<object> IDictionary<string, object>.Values
        {
            get { return ColumnValues; }
        }

        public object[] ColumnValues
        {
            get { return _values; }            
        }

        public IEnumerable<KeyValuePair<string, object>> Items => _columns.Zip(_values, (k, v) => new KeyValuePair<string, object>(k, v));

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _columns;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetValue(binder.Name, out result);
            //result = null;
            //var idx = Array.IndexOf(_columns, binder.Name);
            //if (idx<0) return false;
            //result= _values[idx];
            //return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new InvalidOperationException("This object is read only");
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => this.Items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item) => Items.Contains(item);
        

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            throw new NotImplementedException();
        }

        public int Count => _columns.Length;
        public bool IsReadOnly => true;
    }
}