using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using SqlFu.DDL;
using SqlFu.DDL.Generators.MySql;

namespace SqlFu.Providers
{
    public class MySqlProvider : AbstractProvider
    {
        public const string ProviderName = "MySql.Data.MySqlClient";

        public MySqlProvider(string prov = null) : base(prov ?? ProviderName)
        {
        }

        public override string EscapeName(string s)
        {
            return EscapeIdentifier(s);
        }

        public static string EscapeIdentifier(string s)
        {
            s.MustNotBeEmpty();
            if (s.Contains("`"))
            {
                return s; //already escaped
            }
            if (s.Contains("."))
                return string.Join(".", s.Split('.').Select(d => "`" + d + "`"));
            return "`" + s + "`";
        }

        public override LastInsertId ExecuteInsert(DbCommand cmd, string idKey)
        {
            cmd.CommandText += ";SELECT LAST_INSERT_ID()";
            var rez = cmd.GetRawValue();
            return new LastInsertId(rez);
        }

        public override void SetupParameter(IDbDataParameter param, string name, object value)
        {
            if (value != null)
            {
                if (value is DateTimeOffset)
                {
                    value = value.Cast<DateTimeOffset>().DateTime;
                }
            }
            base.SetupParameter(param, name, value);
        }

        public override DbEngine ProviderType
        {
            get { return DbEngine.MySql; }
        }

        /// <summary>
        /// Gets expression query builder helpers for the rdbms.
        /// Internal usage
        /// </summary>
        public override IDbProviderExpressionHelper BuilderHelper
        {
            get { return new MySqlBuilderHelper(); }
        }

        protected override IDatabaseTools InitTools(SqlFuConnection db)
        {
            return new MysqlDatabaseTools(db);
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + RemoveLastOrderBy(body);
            selecSql = string.Format("{0} limit @{1},@{2}", sql, PreparePagedStatement.SkipParameterName,
                                     PreparePagedStatement.TakeParameterName);
        }

        public override string ParamPrefix
        {
            get { return "?"; }
        }
    }
}