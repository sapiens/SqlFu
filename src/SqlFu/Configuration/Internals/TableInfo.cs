using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SqlFu.Builders;
using SqlFu.Builders.Expressions;
using SqlFu.Mapping;
using SqlFu.Mapping.Internals;
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
                        HasConverter = converter.HasReadConverter(m.PropertyType)                        
                    }).ToArray();
    
        }

        public ColumnInfo this[string propertyName] => Columns.FirstOrDefault(d => d.PropertyInfo.Name == propertyName);

        public string GetIdentityColumnName() => Columns.FirstOrDefault(d => d.IsIdentity)?.Name??null;

        /// <summary>
        /// Returns a dictionary of valid property_name => column_name
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>

        internal IDictionary<string, string> GetColumnNames(IgnoreOn state)
        =>

            Columns.
                Where(d => state switch
                        {
                            IgnoreOn.Read => !d.IgnoreRead,
                            IgnoreOn.Write => !d.IgnoreWrite,
                            _ => true
                        }
                        )
            .ToDictionary(d => d.PropertyInfo.Name, d => d.Name);

		

        public string[] GetColumnNames(IEnumerable<string> properties=null)
            => (properties??Columns.Select(d=>d.PropertyInfo.Name)).Select(p => this[p]).Where(d => !d.IgnoreRead).Select(d => d.Name).ToArray();

        /// <summary>
        /// Used by crud helpers before writing
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
       public object ConvertWriteValue(string propertyName, object value)
        {
            var p=this[propertyName]; 
            return p.ConvertWritableValue(value);
        }

        /// <summary>
        /// Used by reader/mapper
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ConvertReadValue(string propertyName,object value)
		{
            var p = this[propertyName];
            if (!p.HasConverter) return ConvertersManager.DefaultConverter<dynamic>()(value);
            return Converter.ProcessReadValue(p.Type, value);
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
        private TableName _tableName;

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

        public TableName TableName
        {
            get { return _info.TableName; }
            set { _info.TableName = value; }
        }
    }
    enum IgnoreOn { None, Read, Write }
} 