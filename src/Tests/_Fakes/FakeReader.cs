using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace Tests._Fakes
{
    public class FakeReader:DbDataReader
    {
       
        Dictionary<string,object> _values=new Dictionary<string, object>();
            Dictionary<int,string> _names=new Dictionary<int, string>();

        public void Add(string key, object value)
        {
            _values[key] = value;
            if (_names.Any(d=>d.Value==key)) return;
            _names[idx] = key;
            idx++;
            
        }

        public void Clear()
        {
            _values.Clear();
            _names.Clear();
            idx = 0;
        }

        private int idx = 0;

        public override string GetName(int i)
        {
            return _names[i];
        }

        public override string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public override IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override Type GetFieldType(int i)
        {
            return GetValue(i).GetType();
        }

        public override object GetValue(int i)
        {
           return this[GetName(i)];
        }

        public override int GetValues(object[] values)
        {
            Array.Copy(_values.Values.ToArray(),values,_values.Count);
            return _values.Count;
        }

        public override int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public override bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public override byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public override char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public override Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public override short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public override int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public override long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public override float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public override double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public override string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public override decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        

        public override bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public override int FieldCount { get { return _values.Count; } }
        public override bool HasRows { get; }


        public override object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        public override object this[string name]
        {
            get { return _values[name]; }            
        }


        public override bool NextResult()
        {
            throw new NotImplementedException();
        }

        public override bool Read()
        {
            throw new NotImplementedException();
        }

        public override int Depth { get; }
        public override bool IsClosed { get;  }
        public override int RecordsAffected { get;  }
    }
}