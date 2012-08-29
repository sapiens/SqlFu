using System.Data;

namespace SqlFu.Providers
{
    public class PostgresProvider : AbstractProvider
    {
        public const string ProviderName = "Npgsql";
        public PostgresProvider(string provider="Npgsql")
            : base(provider??"Npgsql")
        {

        }
        public override string FormatSql(string sql, params string[] paramNames)
        {
            return sql;
        }

        public override DBType ProviderType
        {
            get { return DBType.PostgreSQL;}
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + body;
            selecSql = string.Format("{0} limit @{2} offset @{1}", sql, PagedSqlStatement.SkipParameterName,
                                     PagedSqlStatement.TakeParameterName);
        }

        public override void SetupParameter(IDbDataParameter param, string name, object value)
        {
            if (value!=null)
            {
                var tp = value.GetType();
                if (tp.IsEnum)
                {
                    value = (int)value;
                }
            }
            
            base.SetupParameter(param, name, value);            
        }

        public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
        {
           if (!string.IsNullOrEmpty(idKey))
           {
               sql.Sql += (" returning "+EscapeName(idKey));
           }
            using(sql)
            {
                return new LastInsertId(sql.ExecuteScalar());
            }
        }

        public override string ParamPrefix
        {
            get { return "@"; }
        }
    }
}