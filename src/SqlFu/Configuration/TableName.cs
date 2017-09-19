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

        /// <summary>
        /// To be used in constraints and indexes definitions
        /// </summary>
        public string DDLUsableString => Schema + Name;

        //todo account for empty schema
        public override string ToString()
        {
            var schema = Schema == null ? "" : Schema + ".";
            return schema + Name;
        }

    }
}