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

        public bool IsIdentity { get; set; }

        public bool IgnoreRead { get; set; }
        public bool IgnoreWrite { get; set; }

        public Type Type => PropertyInfo.PropertyType;
        public string Name { get; set; }

        public Func<dynamic, object> Writer = null;
        

        public bool HasConverter { get; set; }
        public int PocoIdx { get; set; }
    }

    public class ColumnInfo<T> : IConfigurePropertyInfo<T>
    {
        private readonly ColumnInfo _info;

        public ColumnInfo(ColumnInfo info)
        {
            _info = info;
        }

        public IConfigurePropertyInfo<T> IgnoreAtWriting()
        {
            _info.IgnoreWrite = true;
            return this;
        }

        public IConfigurePropertyInfo<T> IsAutoincremented()
        {
            _info.IsIdentity = true;
            return this;
        }

        public IConfigurePropertyInfo<T> BeforeWritingUseConverter(Func<T, object> writer)
        {
            writer.MustNotBeNull();
            _info.Writer = e => writer(e);
            return this;
        }

        public IConfigurePropertyInfo<T> MapToColumn(string name)
        {
            name.MustNotBeEmpty();
            _info.Name = name;
            return this;
        }
    }
}