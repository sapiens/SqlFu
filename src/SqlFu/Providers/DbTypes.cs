using System;
using System.Collections.Generic;

namespace SqlFu.Providers
{
    public class DbTypes : Dictionary<Type, string>
    {
        public DbTypes()
        {
            Add(typeof(int),"int");
            Add(typeof(int?),"int");
            Add(typeof(Single),"real");
            Add(typeof(Single?),"real");
            Add(typeof(Double),"double");
            Add(typeof(Double?),"double");
            Add(typeof(decimal),"decimal");
            Add(typeof(decimal?),"decimal");
            Add(typeof(byte),"smallint");
            Add(typeof(byte?),"smallint");
            Add(typeof(Int16),"smallint");
            Add(typeof(Int16?),"smallint");
            Add(typeof(UInt16),"int");
            Add(typeof(UInt16?),"int");
            Add(typeof(UInt32),"bigint");
            Add(typeof(UInt32?),"bigint");
            Add(typeof(UInt64),"bigint");
            Add(typeof(UInt64?),"bigint");
            Add(typeof(Int64),"bigint");
            Add(typeof(Int64?),"bigint");
            Add(typeof(DateTime),"date");
            Add(typeof(DateTime?),"date");
        }
    }
}