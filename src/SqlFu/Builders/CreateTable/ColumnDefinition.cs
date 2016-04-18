using System;

namespace SqlFu.Builders.CreateTable
{
    public class ColumnDefinition:IConfigureColumn
    {
       
        public string DbType { get; set; }
        public string Definition { get; set; }
        public string Size { get; set; }
        public bool IsNull { get; set; }
        public string Collation { get; set; }
        public string DefaultValue { get; set; }
        public bool IsIdentity { get; set; }

        internal string PropertyName { get; set; }
     

        public Type Type { get; set; }
        public IConfigureColumn HasDbType(string type)
        {
            DbType = type;
            return this;
        }

        public void Define(string def)
        {
            Definition = def;
        }

        public IConfigureColumn HasSize(int size, short? precision = null)
        {
            Size = "(" + size;
            if (precision.HasValue)
            {
                Size += $",{precision.Value}";
            }

            Size += ")";
            return this;
        }

        public IConfigureColumn HasSize(string size)
        {
            Size = $"({size})";
            return this;
        }

        public IConfigureColumn HasCollation(string value)
        {
            Collation = value;
            return this;
        }

        public IConfigureColumn Null()
        {
            IsNull = true;
            return this;
        }

        public IConfigureColumn NotNull()
        {
            IsNull = false;
            return this;
        }

       public IConfigureColumn AutoIncrement()
       {
           IsIdentity = true;
           return this;
       }

        public IConfigureColumn HasDefaultValue(string value, bool valueIsFunction = false)
        {
            if (!valueIsFunction) value = "'" + value + "'";
            DefaultValue = value;
            return this;
        }
    }

    public interface IConfigureColumn
    {
        IConfigureColumn HasDbType(string type);
        /// <summary>
        /// Write the whole column definition yourself
        /// </summary>
        /// <param name="def"></param>
        void Define(string def);
        IConfigureColumn HasSize(int size, short? precision=null);
        IConfigureColumn HasSize(string size);
        IConfigureColumn HasCollation(string value);
        IConfigureColumn Null();
        IConfigureColumn NotNull();
        IConfigureColumn AutoIncrement();
        
        IConfigureColumn HasDefaultValue(string value,bool valueIsFunction=false);
        
        
    }
}