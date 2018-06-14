using System;

namespace SqlFu.Configuration
{
    public class TableName
    {
        public string Name { get; private set; }
        public string Schema { get; private set; }

        internal TableName()
        {
            
        }

        public TableName(string name,string schema=null)
        {
            name.MustNotBeEmpty();            
            Name = name;
            Schema = schema;
        }

       public override string ToString()
        {
            var schema = Schema == null ? "" : Schema + ".";
            return schema + Name;
        }

        public static implicit operator TableName(string d)
        {
            d.MustNotBeEmpty();
            var all = d.Split('.');
            if(all.Length==1) return new TableName(d);
            return new TableName(all[0],all[1]);           
        }
    }
}