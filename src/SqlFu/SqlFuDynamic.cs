using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
namespace SqlFu
{

    public class SqlFuDynamic : DynamicObject
    {
        private readonly string[] _columns;
        private object[] _values;


        public SqlFuDynamic(string[] columns)
        {
            _columns = columns;
            _values = new object[_columns.Length];
        }

        public object[] Values
        {
            get { return _values; }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            var idx = Array.IndexOf(_columns, binder.Name);
            if (idx < 0) return false;
            result = _values[idx];
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            throw new InvalidOperationException("This object is read only");
        }

    }


}

