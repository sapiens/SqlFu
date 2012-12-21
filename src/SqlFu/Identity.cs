using System;
using System.Data;
using System.Text;

namespace SqlFu
{
    public class Identity : IEquatable<Identity>
    {
        private readonly int _hash;

        public Identity(Type t, IDataReader rd, string sql)
        {
            _hash = t.GetHashCode();

            _hash = _hash*17 + sql.GetHashCode();
#if DEBUG
            var sb = new StringBuilder(t.Name);
            sb.AppendFormat("<={0}", sql);
#endif
            unchecked
            {
                for (int i = 0; i < rd.FieldCount; i++)
                {
                    _hash = _hash*23 + rd.GetFieldType(i).GetHashCode();
#if DEBUG
                    //  sb.Append("-" + rd.GetFieldType(i));
#endif
                }
            }
#if DEBUG
            _toString = sb.ToString();
#else
            _toString=_hash.ToString();
#endif
        }

        //public Identity(object[] args)
        //{
        //    _hash = 17;
        //    var sb = new StringBuilder();
        //    unchecked
        //    {
        //        for (int i = 0; i < args.Length; i++)
        //        {
        //            _hash = _hash * 23 + args[i].GetType().GetHashCode();
        //            sb.Append("-" + args[i].GetType());
        //        }
        //    }
        //    _toString = sb.ToString();
        //}

        private readonly string _toString;

        public bool Equals(Identity other)
        {
            return (other != null && _hash == other._hash);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Identity);
        }

        public override string ToString()
        {
            return _toString;
        }

        public override int GetHashCode()
        {
            return _hash;
        }
    }
}