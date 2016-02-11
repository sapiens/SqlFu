using System;
using System.Collections.Generic;
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
                    .Where(m =>m.IsProperty())
                    .Cast<PropertyInfo>()
                    .Select((m,idx) => new ColumnInfo(this, m)
                    {
                        PocoIdx = idx
                        ,HasConverter = converter.HasConverter(m.PropertyType)
                      //  ,CanBeFlattened = !m.PropertyType.IsCustomObjectType() || converter.CanFlattenValueObject(m.PropertyType)
                    }).ToArray();
            HandleAttributeOverride();
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
        
        Dictionary<string,TableSqlCache> _cache=new Dictionary<string, TableSqlCache>();

        public TableSqlCache GetSqlCache(string providerId)
            => _cache.GetValueOrCreate(providerId, () => new TableSqlCache());

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

      

        public TableExistsAction CreationOptions { get; set; }
     

        public override string ToString()
        {
            return $"[Tableinfo]{Type} as {Table}";
        }
      
    }


    public interface ITableInfo
    {
        TableName Table { get; }
        string IdentityColumn { get; set; }
    }
} 