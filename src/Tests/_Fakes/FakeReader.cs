using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Tests._Fakes
{
    public class FakeReader:IDataReader
    {
        public void Dispose()
        {
            
        }
        Dictionary<string,object> _values=new Dictionary<string, object>();
            Dictionary<int,string> _names=new Dictionary<int, string>();

        private void Add(string key, object value)
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

        public string GetName(int i)
        {
            return _names[i];
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public Type GetFieldType(int i)
        {
            return GetValue(i).GetType();
        }

        public object GetValue(int i)
        {
           return this[GetName(i)];
        }

        public int GetValues(object[] values)
        {
            Array.Copy(_values.Values.ToArray(),values,_values.Count);
            return _values.Count;
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public int FieldCount { get { return _values.Count; } }
        

        public object this[int i]
        {
            get { throw new NotImplementedException(); }
        }

        public object this[string name]
        {
            get { return _values[name]; }
            set
            {
                Add(name,value);
            }
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            throw new NotImplementedException();
        }

        public int Depth { get; private set; }
        public bool IsClosed { get; private set; }
        public int RecordsAffected { get; private set; }
    }
}