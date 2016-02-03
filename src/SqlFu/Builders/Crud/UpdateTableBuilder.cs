using System;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders.Expressions;
using SqlFu.Mapping;
using SqlFu.Providers;

namespace SqlFu.Builders.Crud
{
    public class UpdateTableBuilder<T>:IBuildUpdateTable<T>
    {
      
        private readonly HelperOptions _options;
        
        private readonly IExecuteCustomSql _executor;
        private readonly IExpressionWriter _writer;
        private readonly IEscapeIdentifier _utils;
     //   private readonly IManageConverters _converters;
        

        public UpdateTableBuilder(IExecuteCustomSql executor,IExpressionWriter writer,IEscapeIdentifier utils,/*IManageConverters converters,*/HelperOptions options)
        {
            _executor = executor;
            _writer = writer;
            _utils = utils;
          //  _converters = converters;
            _options = options;
            
            _writer.Append($"update {utils.EscapeTableName(options.Table)} set ");
        }

        public IExecuteSql Where(Expression<Func<T, bool>> criteria)
        {
            _writer.SqlBuffer.RemoveLastIfEquals(',');
            _writer.Append(" where ");
            _writer.WriteCriteria(criteria);
            return this;
        }

        CommandConfiguration GetCommandConfig() 
            => new CommandConfiguration(_writer.SqlBuffer.RemoveLastIfEquals(',').ToString(),_writer.Parameters.ToArray()) {ApplyOptions = _options.CmdOptions};

        public int Execute() => _executor.Execute(GetCommandConfig());

        public Task<int> ExecuteAsync(CancellationToken token)=> _executor.ExecuteAsync(GetCommandConfig(), token);
        

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _writer.WriteColumn(column);
            _writer.Append("=");
            _writer.WriteExpression(statement);
            _writer.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            var name = _writer.Helper.GetColumnName(column);
            return Set(name, value);
        }

        public IBuildUpdateTable<T> Set(string propertyName, object value)
        {
            _writer.Append($"{_utils.EscapeIdentifier(propertyName)}=@{_writer.Parameters.CurrentIndex},");
            //_writer.Parameters.AddValues(_converters.ConvertValueObject(value));
            _writer.Parameters.AddValues(value);
            return this;
        }
    }
}