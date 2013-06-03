using System;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using SqlFu.DDL;
using SqlFu.DDL.Generators.SqlServer;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerProvider : AbstractProvider
    {
        private class PagingInfo
        {
            public string CountSql;
            public string SelectSql;
        }

        public const string ProviderName = "System.Data.SqlClient";

        internal SqlServerProvider(string provider) : base(provider)
        {
        }

        public SqlServerProvider() : base(ProviderName)
        {
        }

        public override string FormatSql(string sql, params string[] paramNames)
        {
            return sql;
        }

        public override DbEngine ProviderType
        {
            get { return DbEngine.SqlServer; }
        }

        /// <summary>
        /// Gets expression query builder helpers for the rdbms.
        /// Internal usage
        /// </summary>
        public override IDbProviderExpressionHelper BuilderHelper
        {
            get { return new SqlServerBuilderHelper();}
        }

        protected override IDatabaseTools InitTools(DbAccess db)
        {
            return new SqlServerDatabaseTools(db);
        }


        public override string EscapeName(string s)
        {
            return EscapeIdentifier(s);
        }

        public static string EscapeIdentifier(string s)
        {
            s.MustNotBeEmpty();

            //If the identifier already includes the escape characters, we return
            //the identifier as is.
            if (s.Contains("[") && s.Contains("]"))
                return s;

            //Single part identifier can be returned as is.
            if (!s.Contains(".")) 
                return "[" + s + "]";
            
            //multipart identifier has to be escaped separately.
            return string.Join(".", s.Split('.').Select(d => "[" + d + "]"));
        }

        private static readonly Regex RxOrderBy =
            new Regex(
                @"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*",
                RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<int, PagingInfo> PagingCache= new ConcurrentDictionary<int, PagingInfo>();

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            PagingInfo info;
            var key = sql.GetHashCode();
            if (PagingCache.TryGetValue(key, out info))
            {
                selecSql = info.SelectSql;
                countSql = info.CountSql;
                return;
            }

            int fromidx;
            var body = GetPagingBody(sql, out fromidx);
            selecSql = sql;
            var all = RxOrderBy.Matches(body);
            string orderBy = "order by (select null)";
            if (all.Count > 0)
            {
                var m = all[all.Count - 1];
                orderBy = m.Captures[0].Value;
                body = body.Substring(0, m.Index);
            }
            countSql = "select count(*) " + body;
            var sidx = sql.IndexOf("select", StringComparison.InvariantCultureIgnoreCase);
            if (sidx < 0) throw new InvalidPagedSqlException(sql);
            var columns = sql.Substring(sidx + 7, fromidx - sidx - 7);
            selecSql =
                string.Format(
                    @"SELECT {1} FROM 
(SELECT ROW_NUMBER() OVER ({0}) sqlfu_rn, {1} {2}) 
sqlfu_paged WHERE sqlfu_rn>@{3} AND sqlfu_rn<=(@{3}+@{4})", orderBy, columns, body, PagedSqlStatement.SkipParameterName,
                    PagedSqlStatement.TakeParameterName);
            //cache it
            info = new PagingInfo();
            info.CountSql = countSql;
            info.SelectSql = selecSql;
            PagingCache.TryAdd(key, info);
        }

        public override void SetupParameter(IDbDataParameter param, string name, object value)
        {
            base.SetupParameter(param, name, value);
            if (value == null) return;
            
            var tp = value.GetType();
            if (tp == typeof (string))
            {
                param.Size = Math.Max(((string) value).Length + 1, 4000);
            }

            if (tp.Name == "SqlGeography") //SqlGeography is a CLR Type
            {
                dynamic p = param;
                p.UdtTypeName = "geography";
            }

            else if (tp.Name == "SqlGeometry") //SqlGeometry is a CLR Type
            {
                dynamic p = param;
                p.UdtTypeName = "geometry";
            }
        }

        public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
        {
            sql.Sql += ";Select SCOPE_IDENTITY() as id";

            using (sql)
            {
                var rez = sql.ExecuteScalar();
                return new LastInsertId(rez);
            }
        }

        public const string ParameterPrefix = "@";

        public override string ParamPrefix
        {
            get { return ParameterPrefix; }
        }
    }
}