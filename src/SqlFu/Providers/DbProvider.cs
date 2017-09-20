using System;
using System.Data.Common;
using CavemanTools.Model;
using SqlFu.Mapping;


namespace SqlFu.Providers
{
    using Builders;
    using Configuration;

    public abstract class DbProvider : IDbProvider
    {
        private Func<DbConnection> _factory;

        protected DbProvider(Func<DbConnection> factory,string providerId,SqlFuConfig config=null)
        {
            _factory = factory;
            SqlFuConfiguration = config ?? SqlFuManager.Config;
            ProviderId = providerId;
            EscapeChars = GetEscapeIdentifierChars();
        }

        protected IManageConverters Converters= SqlFuManager.Config.Converters;

        public virtual void SetupParameter(DbParameter param, string name, object value)
        {
            name.MustNotBeEmpty();
            param.ParameterName = ParamPrefix+name;
            
            param.Value = value ?? DBNull.Value;
        }

        private EscapeIdentifierChars EscapeChars;

        protected abstract EscapeIdentifierChars GetEscapeIdentifierChars();

        public string EscapeTableName(TableName table)
        {
            var schema = "";
            if (!table.Schema.IsNullOrEmpty())
            {
                schema = Escape(table.Schema,EscapeChars.Start,EscapeChars.End)+".";
            }

            return schema + Escape(table.Name,EscapeChars.Start, EscapeChars.End);
        }

        public abstract string ParamPrefix { get; }

        public string FormatParameters(string sql, string[] paramNames)
        {
            for (int i = 0; i < paramNames.Length; i++)
            {
                var p = paramNames[i];
                sql = sql.Replace("@" + p, ParamPrefix + p);
            }
            return sql;
        }

        public virtual void OnCommandExecuting(DbCommand cmd)
        {
           
        }

        public string ProviderId { get; }

        public DbConnection CreateConnection()
        {
            return _factory();
        }

        public string EscapeIdentifier(string name)
            => Escape(name, EscapeChars.Start, EscapeChars.End);


        public bool IsTransientError(DbException ex)
        {
            return IsDbBusy(ex);
        }

        public abstract bool IsDbBusy(DbException ex);
        public abstract bool IsUniqueViolation(DbException ex, string keyName = "");
        public abstract bool ObjectExists(DbException ex, string name = null);


        public abstract string AddReturnInsertValue(string sqlValues, string identityColumn);

        public abstract string FormatQueryPagination(string sql, Pagination page, ParametersManager pm);

        private IDbProviderExpressions _expr;

        public void ReplaceExpressionsProvider(IDbProviderExpressions prov)
        {
            prov.MustNotBeNull();
            _expr = prov;
        }

        public IDbProviderExpressions ExpressionsHelper => _expr ?? (_expr = InitExpressionHelper());

        public SqlFuConfig SqlFuConfiguration { get; }

        protected Func<IDbProviderExpressions> InitExpressionHelper=()=>new DbProviderExpressions();

        public static string Escape(string s,char startId,char endId)
        {
            s.MustNotBeEmpty();

            //If the identifier already includes the escape characters, we return
            //the identifier as is.
            if (s[0]==startId && s[s.Length-1]==endId)
                return s;
            if (s.Contains("\""))
                return s;

            if (startId == default(char)) return s;
            //Single part identifier can be returned as is.
            return startId + s + endId;           
        }
    }
}