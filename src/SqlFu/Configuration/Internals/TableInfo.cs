using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SqlFu.Builders;
using SqlFu.Builders.Expressions;
using SqlFu.Mapping;
using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public class TableInfo 
    {
        protected TableInfo()
        {
            
        }
        public TableInfo(Type t, IManageConverters converter, TableName name = null)
        {
            Type = t;
            Converter = converter;
            TableName = name ?? new TableName(t.Name);

            Columns =
                t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.IsProperty())
                    .Cast<PropertyInfo>()
                    .Select((m, idx) => new ColumnInfo(this, m)
                    {
                        PocoIdx = idx,
                        HasConverter = converter.HasConverter(m.PropertyType)

                        //  ,CanBeFlattened = !m.PropertyType.IsCustomObjectType() || converter.CanFlattenValueObject(m.PropertyType)
                    }).ToArray();
            HandleAttributeOverride();
        }

        public ColumnInfo this[string propertyName] => Columns.FirstOrDefault(d => d.PropertyInfo.Name == propertyName);

        public string GetIdentityColumnName() => Columns.FirstOrDefault(d => d.IsIdentity)?.Name??null;

        public void HandleAttributeOverride()
        {
            var attrib = Type.GetSingleAttribute<TableAttribute>(false);

            if (attrib == null) return;
            TableName = new TableName(attrib.Name, attrib.DbSchema);
        }

        public ColumnInfo[] Columns { get; }

        Dictionary<string, TableSqlCache> _cache = new Dictionary<string, TableSqlCache>();
        object _sync = new object();

        public TableSqlCache GetSqlCache(string providerId)
        {
            lock (_sync)
            {
                return _cache.GetValueOrCreate(providerId, () => new TableSqlCache());
            }

        }

        public PagedBuilderResult PagedSql { get; set; }

        public string EscapeName(IEscapeIdentifier provider, TableName name = null)
        {
            name = name ?? TableName;
            return provider.EscapeTableName(name);
        }


        public Type Type { get; internal set; }

        public IManageConverters Converter { get; set; }

        /// <summary>
        /// Gets table name and schema
        /// </summary>
        public TableName TableName { get; set; }


        public override string ToString()
        {
            return $"[Tableinfo]{Type} as {TableName}";
        }

    }


    public interface ITableInfo
    {
        TableName TableName { get; set; }
    }

    public interface IIgnoreColumns<T>
    {
        /// <summary>
        /// Properties will be always ignored read/write
        /// </summary>
        /// <param name="properties"></param>
        void IgnoreProperties(params Expression<Func<T, object>>[] properties);
    }

    public interface ITableInfo<T> : IIgnoreColumns<T>,ITableInfo
    {
        IConfigurePropertyInfo<R> Property<R>(Expression<Func<T, R>> property);
    }

    public class TableInfo<T>:ITableInfo<T>
    {
        private readonly TableInfo _info;

        public TableInfo(TableInfo info)
        {
            _info = info;
        }

        public void IgnoreProperties(params Expression<Func<T, object>>[] properties)
        {
            foreach (var prop in properties)
            {
                var info = _info[prop.GetPropertyName()];
                info.IgnoreWrite= info.IgnoreRead = true;                
            }
        }

        public IConfigurePropertyInfo<R> Property<R>(Expression<Func<T, R>> property)
       =>new ColumnInfo<R>(_info[property.GetPropertyName()]);

        public TableName TableName { get; set; }
    }
} 