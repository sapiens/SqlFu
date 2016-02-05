using System;
using System.Reflection;

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
        //    IsNullable = Type.IsNullable();
            
        }

        public bool IsComplex { get; }
        public PropertyInfo PropertyInfo { get; }

        public Type Type => PropertyInfo.PropertyType;
        public string Name { get; set; }

        public bool CanBeFlattened { get; set; }
        public bool HasConverter { get; set; }
        public int PocoIdx { get; set; }

     
        
     //   public bool IsNullable { get; set; }

   

     
      
      
    }
}