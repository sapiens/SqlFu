using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SqlFu.Builders;
using SqlFu.Builders.CreateTable;
using SqlFu.Mapping;
using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public class TableInfo:ITableInfo
    {
        public TableInfo(Type t, IManageConverters converter)
        {
            Type = t;
            Converter = converter;
            Name = t.Name;
            
            Columns =
                t.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(m => m.MemberType == MemberTypes.Property)
                    .Cast<PropertyInfo>()
                    .Select((m,idx) => new ColumnInfo(this, m) {PocoIdx = idx,HasConverter = converter.HasConverter(m.PropertyType)}).ToArray();
        }

        public void HandleAttributeOverride()
        {
            var attrib = Type.GetCustomAttribute<TableAttribute>(false);
            if (attrib == null) return;
            Name = attrib.Name;
            DbSchema = attrib.DbSchema;
            IdentityColumn = attrib.IdentityColumn;
        }

        public ColumnInfo[] Columns { get; set; }

        public string IdentityColumn { get; set; }
        
        //public TableName TableName { get; set; }=new TableName();
        public TableSqlCache SqlCache { get; private set; }=new TableSqlCache();

        public PagedBuilderResult PagedSql { get; set; }
        public string EscapeName(IEscapeIdentifier provider,string name=null,string schema=null)
        {
            name = name ?? Name;
            schema = schema ?? DbSchema;
            return provider.EscapeTableName(name, schema);
        }
      
      
        public Type Type { get; internal set; }
    
        public IManageConverters Converter { get; set; }
        public string Name { get; set; }

      

        public IfTableExists CreationOptions { get; set; }
        public string DbSchema { get; set; }

     
        //public IEnumerable<ReaderColumn> ReaderColumns
        //{
        //    get
        //    {
        //        if (_readerColumns.Count == 0)
        //        {
        //           FillInReaderColumns();
        //        }
        //        return _readerColumns;
        //    }
        //}

        //public void FillInReaderColumns()
        //{
        //     var i = -1;
        //            Columns.ForEach(c =>
        //            {
        //                i=FillReaderFieldNames(c, _readerColumns,i);
                        
        //            });
        //}
        //private int FillReaderFieldNames(ColumnInfo info, List<ReaderColumn> results, int oldIdx, string prefix = "")
        //{
        //    var result = new ReaderColumn(info);
        //    var resIdx = oldIdx;
        //    result.Name = prefix + info.Name;
        //    result.HasConverter = Converter.HasConverter(info.Type);
        //    result.IsUserMapped = CustomMapper.HasMapper(info.Type);

        //    if (!result.IsComplex || result.HasConverter || result.IsUserMapped)
        //    {
        //        resIdx = oldIdx + 1;
               
        //        result.PocoIdx = resIdx;
        //        results.Add(result);
        //        return resIdx;
        //    }

        //    var name=result.Name+ "_";
            
        //    foreach (var column in Factory.GetColumnsInfo(info.Type))
        //    {
        //        resIdx=FillReaderFieldNames(column, results,resIdx,name);
        //    }
        //    return resIdx;
        //}

        public override string ToString()
        {
            return "[Tableinfo]"+Type.Name;
        }
      
    }

    public class TableSqlCache
    {
        /// <summary>
        /// Cache for insert text
        /// </summary>
        public string InsertSql { get; set; }

        private Lazy<Dictionary<int, PagedSqlCache>> _paged = new Lazy<Dictionary<int, PagedSqlCache>>();
        public PagedSqlCache GetPaged(string sql) => _paged.Value.GetValueOrDefault(sql.GetHashCode());

        public void AddPaged(string sql, PagedSqlCache cache)
        {
            _paged.Value[sql.GetHashCode()] = cache;
        }
    }

    public class PagedSqlCache
    {
        public string CountSql;
        public string PagedSql;
    }


    public interface ITableInfo
    {
        string Name { get; set; }
        string DbSchema { get; set; }
        string IdentityColumn { get; set; }
    }
} 