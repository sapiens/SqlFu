using System;
using System.Data;

namespace SqlFu.Providers
{
    public class SqliteProvider:AbstractProvider
    {
        public const string ProviderName = "System.Data.SQLite";

        public SqliteProvider():this(ProviderName)
        {
            
        }

        public override void SetupParameter(System.Data.IDbDataParameter param, string name, object value)
        {
            base.SetupParameter(param, name, value);
            if (value != null)
            {
                if (value.GetType() == typeof(Guid))
                {
                    param.DbType = DbType.String;
                    value = value.ToString();
                    param.Size = 32;
                }
            }
            
        }

        public SqliteProvider(string providerName) : base(providerName)
        {
        }

        public override string FormatSql(string sql, params string[] paramNames)
        {
            return sql;
        }

        public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
        {
            if (idKey != null)
            {
                sql.Sql += ";SELECT last_insert_rowid()";
                using (sql)
                {
                    var rez = sql.ExecuteScalar();
                    return new LastInsertId(rez);
                }
            }            
            sql.Execute();
            return LastInsertId.Empty;            
        }

        public override string ParamPrefix
        {
            get { return "@"; }
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + body;
            selecSql = string.Format("{0} limit @{1},@{2}", sql, PagedSqlStatement.SkipParameterName,
                                     PagedSqlStatement.TakeParameterName);
        }
    }
}