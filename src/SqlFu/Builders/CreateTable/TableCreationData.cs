using System;
using System.Collections.Generic;
using System.Linq;
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

        public string Name { get; set; }
        public string Schema { get; set; }

        public void Update(TableInfo info)
        {
            if (Name.IsNullOrEmpty())
            {
                Name = info.Name;
            }
            else
            {
                info.Name = Name;
            }

            if (Schema.IsNullOrEmpty())
            {
                Schema = info.DbSchema;
            }
            else
            {
                info.DbSchema = Schema;
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

        public IfTableExists CreationOptions { get; set; }
    }
}