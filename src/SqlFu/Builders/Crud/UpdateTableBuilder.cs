using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;

namespace SqlFu.Builders.Crud
{
    public class UpdateTableBuilder<T>:IBuildUpdateTable<T>
    {
      
        private readonly HelperOptions _options;
        
        private readonly IExecuteCustomSql _executor;
        private readonly IGenerateSqlFromExpressions _writer;
        private readonly IEscapeIdentifier _utils;
     
        StringBuilder _sb=new StringBuilder();

        public UpdateTableBuilder(IExecuteCustomSql executor,IGenerateSqlFromExpressions writer,IEscapeIdentifier utils,HelperOptions options)
        {
            _executor = executor;
            _writer = writer;
            _utils = utils;
        
            _options = options;
            
            _sb.Append($"update {utils.EscapeTableName(options.TableName)} set ");
        }

        public IExecuteSql Where(Expression<Func<T, bool>> criteria)
        {
            _sb.RemoveLastIfEquals(',');
            _sb.Append($" where {_writer.GetSql(criteria)}");
            
            return this;
        }

        public void Where(Expression criteria)
        {
            _sb.RemoveLastIfEquals(',');
            _sb.Append($" where {_writer.GetSql(criteria)}");
        }

        
        CommandConfiguration GetCommandConfig() 
            => new CommandConfiguration(_sb.RemoveLastIfEquals(',').ToString(),_writer.Parameters.ToArray()) {ApplyOptions = _options.CmdOptions};

        public int Execute() => _executor.Execute(GetCommandConfig());

        public Task<int> ExecuteAsync(CancellationToken? token=null)=> _executor.ExecuteAsync(GetCommandConfig(), token);
        

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _sb.Append($"{_writer.GetColumnsSql(column)}={_writer.GetColumnsSql(statement)},");
            
            return this;
        }

        public void SetUpdates(T data)
        {
            foreach (var kv in data.ValuesToDictionary())
            {

                Set(kv.Key, kv.Value);
            }
        }

        private bool _hasWhere = false;
        public void WriteEqualityCriteria(string column, object value)
        {
            if (!_hasWhere)
            {
                _sb.RemoveLastIfEquals(',');
                _sb.Append(" where ");
                _hasWhere = true;
            }
            else
            {
                _sb.Append(" and ");
            }
            _sb.Append($"{_utils.EscapeIdentifier(column)}=@{_writer.Parameters.CurrentIndex},");
            _writer.Parameters.AddValues(value);
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            _writer.EscapeIdentifiers = false;
            var name = _writer.GetColumnsSql(column);
            _writer.EscapeIdentifiers = true;
            return Set(name, value);
        }

        public IBuildUpdateTable<T> Set(string propertyName, object value)
        {
            var name = _options.Info[propertyName].Name;
            _sb.Append($"{_utils.EscapeIdentifier(name)}=@{_writer.Parameters.CurrentIndex},");
            _writer.Parameters.AddValues(_options.Info.ConvertWriteValue(propertyName,value));
            return this;
        }
    }
}