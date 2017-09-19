using System;
using System.Reflection;
using CavemanTools;

namespace SqlFu.Configuration.Internals
{
    public class ColumnInfo
    {
        public TableInfo Table { get; private set; }

        public ColumnInfo(TableInfo table,PropertyInfo propertyInfo)
        {
            Table = table;
            PropertyInfo = propertyInfo;
            Name = propertyInfo.Name;
            IsComplex = Type!=typeof(object) && Type.IsUserDefinedClass();        
            
        }

        public bool IsComplex { get; }
        public PropertyInfo PropertyInfo { get; }

        public Type Type => PropertyInfo.PropertyType;
        public string Name { get; set; }

        public Func<dynamic, object> Writer = d => d;
        public Func<object, dynamic> Reader = d => d;

       public bool HasConverter { get; set; }
        public int PocoIdx { get; set; }
    }
}