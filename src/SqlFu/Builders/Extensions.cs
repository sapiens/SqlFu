using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using CavemanTools;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public static class Extensions
    {
        public static void WriteIfExists(this StringBuilder sb,string feature, ProviderFeatures features )
        {
            if (features.HasOption(feature))
            {
                sb.AppendFormat(" {0}", feature);
            }
        }


        public static void WriteColumns(this StringBuilder builder,IEnumerable<ColumnInfo> columns, IDbProvider provider)
        {
            builder.Append("(");
            columns.ForEach(c => builder.AppendFormat("{0},", provider.EscapeIdentifier(c.Name)));
            builder.RemoveLastIfEquals(",");
            builder.Append(")");
        }

        public static IQuerySql<T> Limit<T>(this ISqlLimit<T> sql, Pagination page)
        {
            return sql.Limit(page.PageSize, page.Skip);
        }

        public static ISqlWhereConnectors<T, TProjection> In<T, TProjection>(this ISqlWhereOptions<T, TProjection> sql,
            Func<DbConnection, IBuildSql> subQuery)
        {
            return sql.In(subQuery(sql.As<QueryBuilder<T, TProjection>>().Cnx));
        }
        public static ISqlWhereConnectors<T, TProjection> WhereExists<T, TProjection>(this ISqlWhere<T, TProjection> sql,
            Func<DbConnection, IBuildSql> subQuery)
        {
            return sql.WhereExists(subQuery(sql.As<QueryBuilder<T, TProjection>>().Cnx));
        }
        public static ISqlWhereConnectors<T, TProjection> NotIn<T, TProjection>(this ISqlWhereOptions<T, TProjection> sql,
            Func<DbConnection, IBuildSql> subQuery)
        {
            return sql.NotIn(subQuery(sql.As<QueryBuilder<T, TProjection>>().Cnx));
        }
        public static ISqlWhereConnectors<T, TProjection> WhereNotExists<T, TProjection>(this ISqlWhere<T, TProjection> sql,
            Func<DbConnection, IBuildSql> subQuery)
        {
            return sql.WhereNotExists(subQuery(sql.As<QueryBuilder<T, TProjection>>().Cnx));
        }
    }
}