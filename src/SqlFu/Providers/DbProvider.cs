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

        protected DbProvider(Func<DbConnection> factory,string providerId)
        {
            _factory = factory;
            ProviderId = providerId;
            _func=new Lazy<DbFunctions>(GetFunctions);
        }

        private Lazy<DbFunctions> _func;

        public DbFunctions Functions => _func.Value;        
        
        /// <summary>
        /// Db specific functions, an inheritor of DbFunctions
        /// </summary>
        /// <returns></returns>
        protected abstract DbFunctions GetFunctions();

        protected IManageConverters Converters= SqlFuManager.Config.Converters;

        public virtual void SetupParameter(DbParameter param, string name, object value)
        {
            name.MustNotBeEmpty();
            param.ParameterName = ParamPrefix+name;
            value = Converters.ConvertValueObject(value);
            param.Value = value ?? DBNull.Value;
        }

        public string EscapeTableName(TableName table)
        {
            var schema = "";
            if (!table.Schema.IsNullOrEmpty())
            {
                schema = EscapeIdentifier(schema)+".";
            }

            return schema + EscapeIdentifier(table.Name);
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

        public abstract string EscapeIdentifier(string name);
        public abstract string GetColumnType(Type type);

      
        private IDatabaseTools _tools;
        public IDatabaseTools DatabaseTools => _tools ?? (_tools = InitTools());

        public virtual string FormatIndexOptions(string idxDef, string options = "") => idxDef;

        public abstract string GetIdentityKeyword();
        public abstract bool IsDbBusy(DbException ex);
        public abstract bool IsUniqueViolation(DbException ex, string keyName = "");
        public abstract bool ObjectExists(DbException ex, string name = null);


        public abstract string AddReturnInsertValue(string values, string identityColumn);

        public abstract string FormatQueryPagination(string sql, Pagination page, ParametersManager pm);

        protected abstract IDatabaseTools InitTools();

        private IDbProviderExpressions _expr;

        public IDbProviderExpressions GetExpressionsHelper()
            => _expr ?? (_expr = InitExpressionHelper());
        protected abstract IDbProviderExpressions InitExpressionHelper();

        public static string Escape(string s,string startId,string endId)
        {
            s.MustNotBeEmpty();

            //If the identifier already includes the escape characters, we return
            //the identifier as is.
            if (s[0]==startId[0] && s[s.Length-1]==endId[0])
                return s;
            if (s.Contains("\""))
                return s;

            //Single part identifier can be returned as is.
            return startId + s + endId;           
        }
    }
}