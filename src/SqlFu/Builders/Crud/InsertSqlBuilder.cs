using System;
using System.Collections.Generic;
using System.Linq;
using SqlFu.Providers;

namespace SqlFu.Builders.Crud
{
    public class InsertSqlBuilder:IGenerateSql
    {
        
        private readonly object _data;
        private readonly IDbProvider _provider;
        private readonly InsertSqlOptions _options;
        
        public InsertSqlBuilder(object data,IDbProvider provider,InsertSqlOptions options)
        {
        
            _data = data;
            _provider = provider;
            _options = options;
             
        }


        public CommandConfiguration GetCommandConfiguration()
        {
            var cache = _options.Info.GetSqlCache(_provider.ProviderId);
            var columnValues =FilterIgnored();
            if (cache.InsertSql.IsNullOrEmpty())
            {
                cache.InsertSql = _provider.CreateInsertSql(_options, columnValues);
            }
            
            return new CommandConfiguration(cache.InsertSql,columnValues.Values.ToArray()) {ApplyOptions = _options.CmdOptions};
        }

        private IDictionary<string, object> FilterIgnored()
        {
            //property name -> column's name 
            var columnValues = _data.ToDictionary().ToDictionary(k=>_options.Info[k.Key].Name,v=> _options.Info.ConvertWriteValue(v.Key, v.Value));
            foreach (var name in GetIgnoredColumns(_options))
            {
                columnValues.Remove(name);
            }            
            return columnValues;
        }
        
        private static IEnumerable<string> GetIgnoredColumns(InsertSqlOptions options) 
            => options.Info.Columns.Where(d=>d.IsComplex || options.IgnoreProperties.Contains(d.PropertyInfo.Name)).Select(d=>d.Name);

       
    }
}