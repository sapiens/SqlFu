using System;
using CavemanTools.Model;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public class PagedQueryBuilder
    {
        private readonly TableInfo _info;
        private readonly IDbProvider _provider;
        
        public PagedQueryBuilder(TableInfo info,IDbProvider provider)
        {
           
            _info = info;
            _provider = provider;
        
        }


        public PagedBuilderResult Build(string sql,object[] args,Pagination page)
        {
            var result=new PagedBuilderResult();
            var provCache = _info.GetSqlCache(_provider.ProviderId);
            var cache = provCache.GetPaged(sql);
            var pm = new ParametersManager(args);
            if (cache == null)
            {
                cache = new PagedSqlCache();
                cache.CountSql = GetCountSql(sql);
                cache.PagedSql = _provider.FormatQueryPagination(sql, page, pm);
                provCache.AddPaged(sql, cache);
            }
            else
            {
                pm.AddValues(page.Skip, page.PageSize);
            }
            result.CountSql = cache.CountSql;
            result.PagedSql = cache.PagedSql;
            result.Args=pm.ToArray();
            return result;
        }

        string GetCountSql(string sql)
        {
            var idxFrom = 0;
            var body = GetPagingBody(sql, out idxFrom);
            var order = body.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
            if (order > -1)
            {
                body = body.Substring(0, order);
            }
            return "select count(*) " + body;
        }

        internal static string GetPagingBody(string sql, out int fromidx)
        {
            fromidx = sql.IndexOf("from", 0, StringComparison.OrdinalIgnoreCase);
            if (fromidx < 0) throw new InvalidPagedSqlException("sql");
            return sql.Substring(fromidx);
        }
    }
}