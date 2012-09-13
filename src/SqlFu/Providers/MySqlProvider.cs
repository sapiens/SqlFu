using System.Data;

namespace SqlFu.Providers
{
    public class MySqlProvider : AbstractProvider
    {
        public const string ProviderName = "MySql.Data.MySqlClient";
        public MySqlProvider(string prov=null) : base(prov??ProviderName)
        {
        }

        public override string EscapeName(string s)
        {
            return "`" + s + "`";
        }
        
        public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
        {
            sql.Sql += ";SELECT LAST_INSERT_ID()";
            using (sql)
            {
                var rez = sql.ExecuteScalar();
                return new LastInsertId(rez);
            }        
        }

        public override void SetupParameter(IDbDataParameter param, string name, object value)
        {
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    value = (int)value;
                }
            }
            base.SetupParameter(param, name, value);
        }

        public override DbEngine ProviderType
        {
            get { return DbEngine.MySql;}
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + body;
            selecSql = string.Format("{0} limit @{1},@{2}", sql, PagedSqlStatement.SkipParameterName,
                                     PagedSqlStatement.TakeParameterName);
        }

        public override string ParamPrefix
        {
            get { return "?"; }
        }
    }
}