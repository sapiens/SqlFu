using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CavemanTools.Model;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public class SimpleSqlBuilder<T>:IWhere<T>
    {
        private readonly HelperOptions _options;
        private readonly IDbProvider _provider;
        private readonly TableInfo _info;
        private readonly ExpressionWriter _writer;
        private StringBuilder _sb;

        public SimpleSqlBuilder(HelperOptions options,IDbProvider provider,ITableInfoFactory infos)
        {
            _options = options;
            _provider = provider;
            _info = infos.GetInfo(typeof(T));
            
            _sb=new StringBuilder();
            var helper=new ExpressionWriterHelper(infos,provider);
            _writer = helper.CreateExpressionWriter(_sb);
            WriteFrom(provider, _info);
            
     
        }

        private void WriteFrom(IDbProvider provider, TableInfo info)
        {
            _sb.Append(
                $" from {provider.EscapeIdentifier(info.EscapeName(provider, _options.TableName, _options.DbSchema))}")
                .AppendLine();
        }

        

        public IGenerateSql<T> AllColumns(bool distinct=false)
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
        {
            return Select<TProj>(_writer.GetSelectColumnsSql(selector), distinct);
        }
        
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
                _sb.Append("order by ");
                ordered = true;
            }
            else
            {
                _sb.Append(",");
            }
            _writer.WriteColumn(column);
            if (!asc) _sb.Append(" desc");
            _sb.AppendLine();
        }

        private bool ordered = false;
        public ISort<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            WriteOrderBy(column,false);
            return this;
        }

        public ISort<T> Where(Expression<Func<T, bool>> criteria)
        {
            _sb.Append("where ");
            _writer.WriteCriteria(criteria);
            _sb.AppendLine();
            return this;
        }

        public ISort<T> Having(Expression<Func<T, bool>> criteria)
        {
            _sb.Append("having ");
            _writer.WriteCriteria(criteria);
            _sb.AppendLine();
            return this;
        }

        public IHaving<T> GroupBy(params Expression<Func<T, object>>[] columns)
        {
            if (columns.Length == 0) return this;
            _sb.Append("group by ");
            columns.Select(c=>_writer.GetExpressionSql(c)).ForEach(t=>_sb.Append($"{t},"));
            _sb.RemoveLast().AppendLine();
            return this;
        }
    }
}