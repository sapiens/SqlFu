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
        private readonly IGenerateSqlFromExpressions _writer;
        private StringBuilder _sb=new StringBuilder();

        public SimpleSqlBuilder(HelperOptions options,IDbProvider provider,TableInfo info,IGenerateSqlFromExpressions writer)
        {
            _options = options;
            _provider = provider;
            _info = info;

            _writer = writer;
            options.EnsureTableName(_info);
            
            WriteFrom(provider, options);
            
     
        }

        private void WriteFrom(IDbProvider provider, HelperOptions info) 
            => _sb.AppendLine($" from {provider.EscapeTableName(info.Table)}");


        public IGenerateSql<T> SelectAll(bool distinct=false,bool useAsterisk=false)
        {
            var columns = "*";
            if (!useAsterisk)
            {
                var sb = new StringBuilder();

                _info.Columns.Select(c => c.Name)
                    .ForEach(n =>
                    {
                        sb.Append($" {_provider.EscapeIdentifier(n)},");
                    });
                columns = sb.RemoveLast().ToString();
            }
           
            return Select<T>(columns, distinct);
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

        public ISelect<T> LimitIf(Func<bool> condition, int take, long offset = 0)
        {
            if (condition()) return Limit(take, offset);
            return this;
        }

        public ISort<T> OrderBy(Expression<Func<T, object>> column)
        {
            WriteOrderBy(column,true);
            return this;
        }

        public ISort<T> OrderByIf(Func<bool> condition, Expression<Func<T, object>> column)
        {
            if (condition()) return OrderBy(column);
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
            _sb.Append(_writer.GetColumnsSql(column));
            
            if (!asc) _sb.Append(" desc");
            _sb.AppendLine();
        }

        private bool ordered = false;
        public ISort<T> OrderByDescending(Expression<Func<T, object>> column)
        {
            WriteOrderBy(column,false);
            return this;
        }

        public ISort<T> OrderByDescendingIf(Func<bool> condition, Expression<Func<T, object>> column)
        {
            if (condition()) return OrderByDescending(column);
            return this;
        }


        public IHaving<T> GroupBy(params Expression<Func<T, object>>[] columns)
        {
            if (columns.Length == 0) return this;
            _sb.Append("group by ");
            _sb.Append(_writer.GetColumnsSql(columns));
          
            _sb.AppendLine();
            return this;
        }

        public IHaving<T> GroupByIf(Func<bool> condition, params Expression<Func<T, object>>[] columns)
        {
            if (condition()) return GroupBy(columns);
            return this;
        }

        IConnectWhere<T> IWhere<T>.Where(Expression<Func<T, bool>> criteria)
        {
            _sb.Append("where ");
            _sb.AppendLine(_writer.GetSql(criteria));
            return this;
        }

        public IConnectWhere<T> WhereIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IWhere<T>).Where(criteria);
            _sb.Append("where 1=1 ");
            return this;
        }

        IConnectHaving<T> IHaving<T>.Having(Expression<Func<T, bool>> criteria)
        {
            _sb.AppendLine($"having {_writer.GetSql(criteria)}");
            
            return this;
        }

        public IConnectHaving<T> HavingIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IHaving<T>).Having(criteria);
            return this;
        }

        IConnectWhere<T> IConnectWhere<T>.And(Expression<Func<T, bool>> criteria)
        {
            _sb.AppendLine($"and {_writer.GetSql(criteria)}");
            
            return this;
        }

        IConnectHaving<T> IConnectHaving<T>.AndIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IConnectHaving<T>).And(criteria);
            return this;
        }

        public IConnectWhere<T> AndIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IConnectWhere<T>).And(criteria);
            return this;
        }

        public IConnectHaving<T> Or(Expression<Func<T, bool>> criteria)
        {
            _sb.AppendLine($"or {_writer.GetSql(criteria)}");
            return this;
        }

        IConnectHaving<T> IConnectHaving<T>.OrIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IConnectHaving<T>).Or(criteria);
            return this;
        }

        public IConnectWhere<T> OrIf(Func<bool> condition, Expression<Func<T, bool>> criteria)
        {
            if (condition()) return (this as IConnectWhere<T>).Or(criteria);
            return this;
        }

        public IConnectHaving<T> And(Expression<Func<T, bool>> criteria)
        {
            _sb.AppendLine($"and {_writer.GetSql(criteria)}");
            return this;
        }

        IConnectWhere<T> IConnectWhere<T>.Or(Expression<Func<T, bool>> criteria)
        {
            _sb.AppendLine($"or {_writer.GetSql(criteria)}");
            return this;
        }
    }
}