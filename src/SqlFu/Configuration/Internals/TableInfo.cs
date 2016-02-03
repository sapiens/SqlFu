using System;
using System.Linq;
using System.Reflection;
using SqlFu.Builders;
using SqlFu.Mapping;
using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public class TableInfo:ITableInfo
    {
        public TableInfo(Type t, IManageConverters converter,TableName name=null)
        {
            Type = t;
            Converter = converter;
            Table = name??new TableName(t.Name);
            
            Columns =
                t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsProperty())
                    .Cast<PropertyInfo>()
                    .Select((m,idx) => new ColumnInfo(this, m) {PocoIdx = idx,HasConverter = converter.HasConverter(m.PropertyType)}).ToArray();
        }

        public void HandleAttributeOverride()
        {
            var attrib = Type.GetSingleAttribute<TableAttribute>(false);
            
            if (attrib == null) return;
            Table = new TableName(attrib.Name,attrib.DbSchema);
            
            IdentityColumn = attrib.IdentityColumn;
        }

        public ColumnInfo[] Columns { get; set; }

        public string IdentityColumn { get; set; }
        
        public TableSqlCache SqlCache { get; private set; }=new TableSqlCache();

        public PagedBuilderResult PagedSql { get; set; }
        public string EscapeName(IEscapeIdentifier provider,TableName name=null)
        {
            name = name ?? Table;
            return provider.EscapeTableName(name);
        }
      
      
        public Type Type { get; internal set; }
    
        public IManageConverters Converter { get; set; }

        /// <summary>
        /// Gets table name and schema
        /// </summary>
        public TableName Table { get; internal set; }

      

        public IfTableExists CreationOptions { get; set; }
     

        public override string ToString()
        {
            return "[Tableinfo]"+Type.Name;
        }
      
    }


    public interface ITableInfo
    {
        TableName Table { get; }
        string IdentityColumn { get; set; }
    }
} 