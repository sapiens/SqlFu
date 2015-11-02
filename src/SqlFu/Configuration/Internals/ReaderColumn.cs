using System;
using System.Reflection;

namespace SqlFu.Configuration.Internals
{
    public class ReaderColumn
    {
        public ReaderColumn(ColumnInfo info)
        {
            Info = info;

            Type = info.Type;
            IsComplex = Type.IsUserDefinedClass();
            PropertyInfo = info.PropertyInfo;
        }

        public PropertyInfo PropertyInfo { get; set; }

        public Type Type { get; set; }
        public string Name { get; set; }
        public int PocoIdx { get; set; }
        public ColumnInfo Info { get; set; }
        public bool IsComplex { get; private set; }
        public bool HasConverter { get; set; }
        //public bool IsUserMapped { get; set; }
    }
}