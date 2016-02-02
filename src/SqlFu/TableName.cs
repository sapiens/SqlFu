using System;

namespace SqlFu
{
    public class TableName
    {
        public string Name { get; private set; }
        public string Schema { get; private set; }

        internal TableName()
        {
            
        }

        public TableName(string name,string schema="")
        {
            name.MustNotBeEmpty();            
            Name = name;
            Schema = schema??"";
        }

        /// <summary>
        /// To be used in constraints and indexes definitions
        /// </summary>
        public string DDLUsableString => Schema + Name;

        public override string ToString() => Schema + "." + Name;

    }
}