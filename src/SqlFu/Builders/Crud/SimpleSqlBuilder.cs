using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CavemanTools.Model;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.Crud
{
    public class SimpleSqlBuilder<T>:IWhere<T>,IConnectWhere<T>,IConnectHaving<T>
    {
        private readonly HelperOptions _options;
        private readonly IDbProvider _provider;
        private readonly TableInfo _info;
        private readonly IExpressionWriter _writer;
        private StringBuilder _sb;

        public SimpleSqlBuilder(HelperOptions options,IDbProvider provider,TableInfo info,IExpressionWriter writer)
        {
            _options = options;
            _provider = provider;
            _info = info;

            _writer = writer;
            _sb = _writer.SqlBuffer;
            
            WriteFrom(provider, _info);
            
     
        }

        private void WriteFrom(IDbProvider provider, TableInfo info) 
            => _writer.Append($" from {provider.EscapeTableName(info.Table)}",true);


        public IGenerateSql<T> SelectAll(bool distinct=false)
        {
            var sb=new StringBuilder();
            _info.Columns.Select(c=>c.Name)
                .ForEach(n =>
                {
                    sb.Append($" {_provider.EscapeIdentifier(n)},");
                });
            return Select<T>(sb.RemoveLast().ToString(), distinct);
        }

        BuiltSql<TResult> Select<TResult>(string columns,bool distinct)
        {
            var text = "select ";
            if (distinct) text += "distinct ";
            text += columns;
            _sb.Insert(0, text);
            return new BuiltSql<TResult>(_sb.ToString(), _writer.Parameters.ToArray(),_options);
        }

        public IGenerateSql<TProj> Select<TProj>(Expression<Func<T, TProj>> selector, bool distinct = false) 
            => Select<TProj>(_writer.GetColumnsSql(selector), distinct);

        public ISelect<T> Limit(int take, long offset = 0)
        {
            _sb=new StringBuilder(_provider.FormatQueryPagination(_sb.ToString(), Pagination.Create(offset,take), _writer.Parameters));
            return this;
        }

        public ISort<T> OrderBy(Expression<Func<T, object>> column)
        {
            WriteOrderBy(column,true);
            return this;
        }

        void WriteOrderBy(LambdaExpression column, bool asc)
        {
            if (!ordered)
            {
                _writer.Append("order by ");
                ordered = true;
            }
            else
            {
                _writer.Append(",");
            }
            _writer.WriteColumn(column);
            if (!asc) _writer.Append(" desc");
            _sb.AppendLine();
        }

        private bool ordered = false;
        public ISort<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            WriteOrderBy(column,false);
            return this;
        }

       
        public IHaving<T> GroupBy(params Expression<Func<T, object>>[] columns)
        {
            if (columns.Length == 0) return this;
            _writer.Append("group by ");
            columns.Select(c=>_writer.GetExpressionSql(c)).ForEach(t=>_writer.Append($"{t},"));
            _sb.RemoveLast().AppendLine();
            return this;
        }

        IConnectWhere<T> IWhere<T>.Where(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("where ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }

        IConnectHaving<T> IHaving<T>.Having(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("having ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }

        IConnectWhere<T> IConnectWhere<T>.And(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("and ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }

        public IConnectHaving<T> Or(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("or ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }

        public IConnectHaving<T> And(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("and ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }

        IConnectWhere<T> IConnectWhere<T>.Or(Expression<Func<T, bool>> criteria)
        {
            _writer.Append("or ");
            _writer.WriteCriteria(criteria);
            _writer.AppendLine();
            return this;
        }
    }
}