using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.Crud
{
    public class InsertSqlBuilder:IGenerateSql
    {
        private readonly TableInfo _info;
        private readonly object _data;
        private readonly IDbProvider _provider;
        private readonly InsertSqlOptions _options;

        public InsertSqlBuilder(TableInfo info,object data,IDbProvider provider,InsertSqlOptions options)
        {
            _info = info;
            _data = data;
            _provider = provider;
            _options = options;            
        }


        public CommandConfiguration GetCommandConfiguration()
        {
            if (_info.SqlCache.InsertSql.IsNullOrEmpty())
            {
                _info.SqlCache.InsertSql = Build();
            }

            return new CommandConfiguration(_info.SqlCache.InsertSql,GetValues()) {ApplyOptions = _options.CmdOptions};
        }

        
        private string Build()
        {
            var columns = GetInsertColumns();
            var values = _provider.AddReturnInsertValue(GetValuesPlaceholders(),_options.IdentityColumn);
            return $"{columns} \n {values}";
        }

        private object[] GetValues() => GetInsertableColumns().Select(c => _data.GetPropertyValue(c)).ToArray();

        private string GetInsertColumns()
        {
            var builder = new StringBuilder();
            if (_options.IdentityColumn.IsNullOrEmpty()) _options.IdentityColumn = _info.IdentityColumn;
            builder.Append($"insert into {_info.EscapeName(_provider, _options.Table)} (");

            GetInsertableColumns()
                 .ForEach(n =>
                 {
                     builder.Append($"{ _provider.EscapeIdentifier(n)},");
                 });

            builder.RemoveLastIfEquals(',').Append(")");
            return builder.ToString();
        }

        private IEnumerable<string> GetInsertableColumns() 
            => _info.Columns.Select(d => d.Name).Where(c => c != _options.IdentityColumn && !_options.IgnoreColumns.Contains(c));

        private string GetValuesPlaceholders()
        {
            var sb = new StringBuilder();
            sb.Append("values(");

            GetInsertableColumns().ForEach((idx, name) => sb.Append($"@{idx},"));

            return sb.RemoveLastIfEquals(',').Append(")").ToString();
        }
      
    }
}