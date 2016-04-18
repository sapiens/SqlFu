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
            
            _sb.Append($"update {utils.EscapeTableName(options.Table)} set ");
        }

        public IExecuteSql Where(Expression<Func<T, bool>> criteria)
        {
            _sb.RemoveLastIfEquals(',');
            _sb.Append($" where {_writer.GetSql(criteria)}");
            
            return this;
        }

        CommandConfiguration GetCommandConfig() 
            => new CommandConfiguration(_sb.RemoveLastIfEquals(',').ToString(),_writer.Parameters.ToArray()) {ApplyOptions = _options.CmdOptions};

        public int Execute() => _executor.Execute(GetCommandConfig());

        public Task<int> ExecuteAsync(CancellationToken token)=> _executor.ExecuteAsync(GetCommandConfig(), token);
        

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _sb.Append($"{_writer.GetColumnsSql(column)}={_writer.GetColumnsSql(statement)},");
            
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            var name = _writer.GetColumnsSql(column);
            return Set(name, value);
        }

        public IBuildUpdateTable<T> Set(string propertyName, object value)
        {
            _sb.Append($"{_utils.EscapeIdentifier(propertyName)}=@{_writer.Parameters.CurrentIndex},");
            _writer.Parameters.AddValues(value);
            return this;
        }
    }
}