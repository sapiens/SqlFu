using System;
using System.Collections.Generic;
using System.Linq;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders.CreateTable
{
    internal class TableCreationData
    {
        public Type Type { get; set; }

        public TableCreationData(Type type)
        {
            Type = type;
        }

        public TableName TableName { get; internal set; }
        
        public void Update(TableInfo info)
        {
            if (TableName == null)
            {
                TableName = info.Table;
            }
            else
            {
                info.Table = TableName;
            }
           
            var auto = Columns.Find(d => d.IsIdentity);
            if (auto != null)
            {
                info.IdentityColumn = auto.PropertyName;
            }
        }

        public List<ColumnDefinition> Columns { get; } = new List<ColumnDefinition>();

        public ColumnDefinition Column(string name) => Columns.FirstOrDefault(d => d.PropertyName == name);

        public List<IndexDefinition> Indexes { get; }=new List<IndexDefinition>();

        public PKData PrimaryKey { get; set; }

        public List<ForeignKeyDefinition> ForeignKeys { get; }=new List<ForeignKeyDefinition>();

        public Actions CreationOptions { get; set; }
    }
}