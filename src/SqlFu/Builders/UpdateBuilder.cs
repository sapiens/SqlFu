using System;
using System.Data.Common;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders.Expressions;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public class UpdateBuilder<T>:IBuildUpdateTable<T>
    {
        private readonly DbConnection _db;
        private readonly HelperOptions _options;
        private StringBuilder _sb;
        private ExpressionWriter _writer;
        private IDbProvider _provider;

        public UpdateBuilder(DbConnection db,HelperOptions options)
        {
            _db = db;
            _provider = db.GetProvider();
            _options = options;
            _sb =new StringBuilder();
            _writer = db.CreateWriterHelper().CreateExpressionWriter(_sb);
        }

        public IExecuteSql Where(Expression<Func<T, bool>> criteria)
        {
            _sb.RemoveLastIfEquals(',');
            _sb.Append(" where ");
            _writer.WriteCriteria(criteria);
            return this;
        }

        CommandConfiguration GetCommandConfig() => new CommandConfiguration("update "+_db.GetPocoInfo<T>().EscapeName(_provider,_options.Table)+" set " +_sb.RemoveLastIfEquals(','),_writer.Parameters.ToArray()) {ApplyOptions = _options.CmdOptions};

        public int Execute() => _db.Execute(GetCommandConfig());

        public Task<int> ExecuteAsync(CancellationToken token)=> _db.ExecuteAsync(GetCommandConfig(), token);
        

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _writer.WriteColumn(column);
            _sb.Append("=");
            _writer.WriteExpression(statement);
            _sb.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            var name = _writer.Helper.GetColumnName(column);
            return Set(name, value);
        }

        public IBuildUpdateTable<T> Set(string propertyName, object value)
        {
            _sb.Append($"{_provider.EscapeIdentifier(propertyName)}=@{_writer.Parameters.CurrentIndex},");
            var voMapper = SqlFuManager.Config.Converters;
            _writer.Parameters.AddValues(voMapper.ConvertValueObject(value));
            return this;
        }
    }
}