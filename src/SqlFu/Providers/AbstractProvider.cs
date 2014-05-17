using System;
using System.Data;
using System.Data.Common;
using SqlFu.DDL;

namespace SqlFu.Providers
{
    public abstract class AbstractProvider : IHaveDbProvider
    {
        private readonly DbProviderFactory _factory;

        protected internal AbstractProvider(string providerName)
        {
            _factory = DbProviderFactories.GetFactory(providerName);
        }

        public virtual string EscapeName(string s)
        {
            return "\"" + s + "\"";
        }

        public abstract LastInsertId ExecuteInsert(DbCommand cmd, string idKey);


        public DbConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }


        protected string RemoveLastOrderBy(string body)
        {
            var pos = body.LastIndexOf("order", StringComparison.OrdinalIgnoreCase);
            if (pos < 0) return body;
            return body.Substring(0, pos);
        }

        internal static string GetPagingBody(string sql, out int fromidx)
        {
            fromidx = sql.IndexOf("from", 0, StringComparison.InvariantCultureIgnoreCase);
            if (fromidx < 0) throw new InvalidPagedSqlException("sql");
            return sql.Substring(fromidx);
        }


        public abstract void MakePaged(string sql, out string selecSql, out string countSql);

        public virtual void SetupParameter(IDbDataParameter param, string name, object value)
        {
            if (name == null) name = "";
            param.ParameterName = string.Concat(ParamPrefix, name);
            if (value != null)
            {
                if (value.GetType().IsEnum)
                {
                    param.Value = value.ConvertTo(Enum.GetUnderlyingType(value.GetType()));
                }
                else
                {
                    param.Value = value;
                }
            }
            else
            {
                param.Value = DBNull.Value;
            }
        }

        public abstract string ParamPrefix { get; }

        public virtual string FormatSql(string sql, params string[] paramNames)
        {
            Array.ForEach(paramNames, p => { sql = sql.Replace("@" + p, ParamPrefix + p); });
            return sql;
        }

        public virtual void OnCommandExecuting(IDbCommand cmd)
        {
        }

        public abstract DbEngine ProviderType { get; }
        private IDatabaseTools _tools;

        public IDatabaseTools GetTools(SqlFuConnection db)
        {
            if (_tools == null)
            {
                _tools = InitTools(db);
            }
            return _tools;
        }

        /// <summary>
        /// Gets expression query builder helpers for the rdbms.
        /// Internal usage
        /// </summary>
        public abstract IDbProviderExpressionHelper BuilderHelper { get; }


        protected abstract IDatabaseTools InitTools(SqlFuConnection db);
    }
}