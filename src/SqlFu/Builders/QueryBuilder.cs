using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model;
using SqlFu.Builders.Internals.SqlParts;

namespace SqlFu.Builders
{
    public class QueryBuilder<T, TProjection>:ASqlBuilder,ISqlFrom<T,TProjection>,ISqlWhereConnectors<T,TProjection>,ISqlWhereOptions<T,TProjection>
    {
        
        private readonly DbConnection _cnx;

        public QueryBuilder(SqlQueryManager manager,DbConnection cnx) : base(manager)
        {
            _cnx = cnx;
            //we want T to always be t0
            Manager.GetTableAlias(typeof (T));
        }

        public void SelectColumns()
        {
            Parts.Add(Select,"*");
        }

        public void SelectColumns(LambdaExpression projection)
        {
            Parts.Add(new SqlBuilderPartSelectColumnsExpression(Select,projection,ExpressionWriter));
        }
    
        protected override void BuildParts()
        {
            Builder.Append("select ");
            if (_selectDistinct) Builder.Append("distinct ");
            var cols = Parts.GetParts(s => s == Select).First();
            Builder.Append(cols);
            Builder.AppendLine();
            WriteFrom<T>();
            Parts.GetParts(s=>s.Contains("join")).ForEach(p=>Builder.AppendFormat("\n{0}",p.ToString()));
            
         
            WriteCriteria();
            
            WriteGroupBy();

            WriteHaving();

            WriteOrderBy();

            WriteLimit();
        }

        private void WriteLimit()
        {
            var part = Parts.GetParts(s => s == Limit_).FirstOrDefault();
            if (part==null) return;
            var data = part as SqlBuilderPartLimit;
            Builder.AppendLine();
            var sql = Builder.ToString();
            Builder.Clear();
            Builder.Append(Manager.Provider.FormatQueryPagination(data.Take, data.Skip, Manager.Parameters,
                sql));
        }

        private void WriteOrderBy()
        {
            var parts = Parts.GetParts(s => s == OrderBy_);
            if (parts.Length == 0) return;
            Builder.AppendLine();
            Builder.Append("order by");
            parts.ForEach(d => Builder.AppendFormat(" {0},", d));
            Builder.RemoveLast();
        }

        private void WriteHaving()
        {
            WriteCriteria(new []{Having_,HavingAnd,HavingOr});
        }

        private void WriteGroupBy()
        {
            var g = Parts.GetParts(s => s == Group_By);
            if (g.Length == 0) return;
            Builder.AppendLine();
            Builder.AppendFormat("group by {0}", g[0].ToString());
        }

        public async Task<IEnumerable<TProjection>> GetRowsAsync(CancellationToken token)
        {
            return await GetRowsAsync<TProjection>(token).ConfigureAwait(false);
        }

        public Task<PagedResult<TProjection>> GetPagedAsync(CancellationToken token)
        {
            return GetPagedAsync<TProjection>(token);
        }

        public Task<PagedResult<TOther>> GetPagedAsync<TOther>(CancellationToken token)
        {
            var part = Parts.GetParts(d => d == Limit_).FirstOrDefault();
            if (part == null) throw new InvalidOperationException("Can paginate this query. You need to specify pagination using the Limit method");

            Parts.Remove(part);
            var limit = part as SqlBuilderPartLimit;
            return Cnx.QueryPagedAsync<TOther>(limit.Take, limit.Skip, token, this);
        }

        public async Task<IEnumerable<TOther>> GetRowsAsync<TOther>(CancellationToken token)
        {
            using (var cmd = Cnx.CreateAndSetupCommand(GetSql(), GetArguments()))
            {
                return await cmd.FetchAsync<TOther>(CancellationToken.None).ConfigureAwait(false);
            }   
        }

        public async Task<TProjection> GetValueAsync(CancellationToken token)
        {
            using (var cmd = Cnx.CreateAndSetupCommand(GetSql(), GetArguments()))
            {
                return await cmd.GetValueAsync<TProjection>(CancellationToken.None).ConfigureAwait(false);
            }
        }

        public IEnumerable<TProjection> GetRows()
        {
            return GetRows<TProjection>();
        }

        public PagedResult<TProjection> GetPaged()
        {
            return GetPaged<TProjection>();
        }

        public PagedResult<TOther> GetPaged<TOther>()
        {
            var part = Parts.GetParts(d => d == Limit_).FirstOrDefault();
            if (part == null) throw new InvalidOperationException("Can't paginate this query. You need to specify pagination using the Limit method");

            Parts.Remove(part);
            var limit = part as SqlBuilderPartLimit;
            return Cnx.QueryPaged<TOther>(limit.Take, limit.Skip, this);
        }

        public IEnumerable<TOther> GetRows<TOther>()
        {
            using (var cmd = Cnx.CreateAndSetupCommand(GetSql(), GetArguments()))
            {
                return cmd.Fetch<TOther>();
            }   
        }

        public TProjection GetValue()
        {
            using (var cmd = Cnx.CreateAndSetupCommand(GetSql(), GetArguments()))
            {
                return cmd.GetValue<TProjection>();
            }
        }

        public IQuerySql<TProjection> Limit(int take, long skip = 0)
        {
            Parts.Add(new SqlBuilderPartLimit(take,skip));
            return this;
        }

        public ISqlOrder<T, TProjection> OrderBy(Expression<Func<TProjection, object>> criteria)
        {
            Parts.Add(new SqlBuilderPartOrderBy(criteria,Builders.OrderBy.Asc,ExpressionWriter));
            return this;
        }

        public ISqlOrder<T, TProjection> OrderBy<TOther>(Expression<Func<TOther, object>> criteria)
        {
            Parts.Add(new SqlBuilderPartOrderBy(criteria, Builders.OrderBy.Asc, ExpressionWriter));
            return this;
        }

        public ISqlOrder<T, TProjection> OrderByDescending(Expression<Func<TProjection, object>> criteria)
        {
            Parts.Add(new SqlBuilderPartOrderBy(criteria, Builders.OrderBy.Desc, ExpressionWriter));
            return this;
        }

        public ISqlOrder<T, TProjection> OrderByDescending<TOther>(Expression<Func<TOther, object>> criteria)
        {
            Parts.Add(new SqlBuilderPartOrderBy(criteria, Builders.OrderBy.Desc, ExpressionWriter));
            return this;
        }

        public ISqlHavingConnectors<T, TProjection> Having(Expression<Func<T, bool>> criteria)
        {
            Parts.Add(Having_,criteria);
            return new HavingConnector(this);
        }

        public ISqlHavingConnectors<T, TProjection> Having<TOther>(Expression<Func<TOther, bool>> criteria)
        {
            Parts.Add(Having_, criteria);
            return new HavingConnector(this);
        }


        class HavingConnector:ISqlHavingConnectors<T,TProjection>
        {
            private readonly QueryBuilder<T, TProjection> _parent;

            public HavingConnector(QueryBuilder<T,TProjection> parent)
            {
                _parent = parent;
            }

            public IQuerySql<TProjection> Limit(int take, long skip = 0)
            {
                return _parent.Limit(take, skip);
            }

            public ISqlOrder<T, TProjection> OrderBy(Expression<Func<TProjection, object>> criteria)
            {
                return _parent.OrderBy(criteria);
            }

            public ISqlOrder<T, TProjection> OrderBy<TOther>(Expression<Func<TOther, object>> criteria)
            {
                return _parent.OrderBy<TOther>(criteria);
            }

            public ISqlOrder<T, TProjection> OrderByDescending(Expression<Func<TProjection, object>> criteria)
            {
                return _parent.OrderByDescending(criteria);
            }

            public ISqlOrder<T, TProjection> OrderByDescending<TOther>(Expression<Func<TOther, object>> criteria)
            {
                return _parent.OrderByDescending(criteria);
            }

            public string GetSql()
            {
                return _parent.GetSql();
            }

            public object[] GetArguments()
            {
                return _parent.GetArguments();
            }

            public Task<IEnumerable<TProjection>> GetRowsAsync(CancellationToken token)
            {
                return _parent.GetRowsAsync(token);
            }

            public Task<PagedResult<TProjection>> GetPagedAsync(CancellationToken token)
            {
                return _parent.GetPagedAsync(token);
            }

            public Task<PagedResult<TOther>> GetPagedAsync<TOther>(CancellationToken token)
            {
                return _parent.GetPagedAsync<TOther>(token);
            }

            public Task<IEnumerable<TOther>> GetRowsAsync<TOther>(CancellationToken token)
            {
                return _parent.GetRowsAsync<TOther>(token);
            }

            public Task<TProjection> GetValueAsync(CancellationToken token)
            {
                return _parent.GetValueAsync(token);
            }

            public IEnumerable<TProjection> GetRows()
            {
                return _parent.GetRows();
            }

            public PagedResult<TProjection> GetPaged()
            {
                return _parent.GetPaged();
            }

            public PagedResult<TOther> GetPaged<TOther>()
            {
                return _parent.GetPaged<TOther>();
            }

            public IEnumerable<TOther> GetRows<TOther>()
            {
                return _parent.GetRows<TOther>();
            }

            public TProjection GetValue()
            {
                return _parent.GetValue();
            }

            public ISqlHavingConnectors<T, TProjection> Having(Expression<Func<T, bool>> criteria)
            {
                return _parent.Having(criteria);
            }

            public ISqlHavingConnectors<T, TProjection> Having<TOther>(Expression<Func<TOther, bool>> criteria)
            {
                return _parent.Having(criteria);
            }

            public ISqlHaving<T, TProjection> And
            {
                get
                {
                    _parent.Parts.Add(HavingAnd,"and");
                    return this;
                }
            }
            public ISqlHaving<T, TProjection> Or {
                get
                {
                    _parent.Parts.Add(HavingOr, "or");
                    return this;
                }
            }
        }

        public ISqlGroup<T, TProjection> GroupBy(params Expression<Func<TProjection, object>>[] expression)
        {
            Parts.Add(new SqlBuilderPartColumnsNamesExpression(Group_By,ExpressionWriter, expression));
            return this;
        }

        public ISqlWhereConnectors<T, TProjection> Where(Expression<Func<T, bool>> criteria)
        {
            Parts.Add(WhereEmpty, criteria);
         
            return this;
        }

        public ISqlWhereConnectors<T, TProjection> Where<TOther>(Expression<Func<TOther, bool>> criteria)
        {
            Parts.Add(WhereEmpty, criteria);
         
            return this;
        }

        public ISqlWhereOptions<T, TProjection> Where(Expression<Func<T, object>> column)
        {
            _currentWhereProperty = column;
            return this;
        }

        private int _subqueryCount;

        public ISqlWhereConnectors<T, TProjection> WhereExists(IBuildSql subquery)
        {
            _subqueryCount++;
            Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(Where_Exists,"exists"), _currentWhereProperty, subquery,_subqueryCount, ExpressionWriter, Parameters));            
            return this;
        }

        public ISqlWhereConnectors<T, TProjection> WhereNotExists(IBuildSql subquery)
        {
            _subqueryCount++;
            Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(Where_NotExists, "not exists"), _currentWhereProperty, subquery, _subqueryCount, ExpressionWriter, Parameters));
            return this;
        }

        private bool _selectDistinct;
        public ISqlFrom<T, TProjection> Distinct()
        {
            _selectDistinct = true;
            return this;
        }

        public ISqlFrom<T, TProjection> InnerJoin<TOther>(Expression<Func<T, TOther, bool>> criteria)
        {
            var table = Manager.FormatTableName(typeof (TOther));
            Parts.Add(new SqlBuilderPartJoin(JoinInner,table,criteria,ExpressionWriter));
            return this;
        }

        public ISqlFrom<T, TProjection> LeftJoin<TOther>(Expression<Func<T, TOther, bool>> criteria)
        {
            var table = Manager.FormatTableName(typeof(TOther));
            Parts.Add(new SqlBuilderPartJoin(JoinOuterLeft, table, criteria, ExpressionWriter));
            return this;
        }

      
        private LambdaExpression _currentWhereProperty = null;

        public ISqlWhere<T, TProjection> And
        {
            get { 
               Parts.Add(WhereAnd,"and");
              return this;
            }
        }

        public ISqlWhere<T, TProjection> Or
        {
            get
            {
                Parts.Add(WhereOr,"or");
               return this;
            }
        }

        public DbConnection Cnx
        {
            get { return _cnx; }
        }

        public ISqlWhereConnectors<T, TProjection> In(IBuildSql subquery)
        {
            _subqueryCount++;
            Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(WhereIn, "in"), _currentWhereProperty, subquery, _subqueryCount, ExpressionWriter, Parameters));
            _currentWhereProperty = null;
            return this;
        }

        public ISqlWhereConnectors<T, TProjection> NotIn(IBuildSql subquery)
        {
            _subqueryCount++;
            Parts.Add(new SqlBuilderPartSubQuery(new SqlBuilderPartValue(WhereNotIn, "not in"), _currentWhereProperty, subquery, _subqueryCount, ExpressionWriter, Parameters));
            _currentWhereProperty = null;
            return this;
        }

    
    }
}