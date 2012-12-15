using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Reflection;
using SqlFu.DDL;

namespace SqlFu.Providers
{
    public abstract class AbstractProvider:IHaveDbProvider
    {
        private readonly DbProviderFactory _factory;

        internal protected AbstractProvider(string providerName)
        {
            _factory=DbProviderFactories.GetFactory(providerName); 
        }

        public virtual string EscapeName(string s)
        {
            return "\"" + s + "\"";
        }

        public abstract LastInsertId ExecuteInsert(SqlStatement sql, string idKey);


        public DbConnection CreateConnection()
        {
            return _factory.CreateConnection();
        }

        //internal static string GetColumnsFromSql(string sql)
        //{
        //    var sidx = sql.IndexOf("select", 0, StringComparison.InvariantCultureIgnoreCase);
        //    if (sidx<0) throw new InvalidPagedSqlException(sql);
        //    sidx = sidx + 7;
        //    var fromidx = sql.IndexOf("from", sidx, StringComparison.InvariantCultureIgnoreCase);
        //    if (fromidx < 0) fromidx = sql.Length;
        //    return sql.Substring(sidx, fromidx - sidx);
        //}

        internal static string GetPagingBody(string sql, out int fromidx)
        {
            fromidx = sql.IndexOf("from", 0, StringComparison.InvariantCultureIgnoreCase);
            if (fromidx<0) throw new InvalidPagedSqlException("sql");
            return sql.Substring(fromidx);
        }

        

        public abstract void MakePaged(string sql, out string selecSql, out string countSql);

        public virtual void SetupParameter(IDbDataParameter param, string name, object value)
        {
            if (name == null) name = "";
            param.ParameterName = ParamPrefix + name;
            if (value!=null)
            {
                param.Value = value;
            }
            else
            {
                param.Value = DBNull.Value;
            }
           
        }

        public abstract string ParamPrefix { get; }

        public virtual string FormatSql(string sql, params string[] paramNames)
        {
            Array.ForEach(paramNames,p=>
                                         {
                                             sql=sql.Replace("@" + p, ParamPrefix + p);
                                         });            
            return sql;

        }

        public virtual void OnCommandExecuting(IDbCommand cmd)
        {
            
        }

        public abstract DbEngine ProviderType { get; }
        private IDatabaseTools _tools;
        public IDatabaseTools GetTools(DbAccess db)
        {
        
                if (_tools==null)
                {
                    _tools = InitTools(db);
                }
                return _tools;
         
        }

        protected abstract IDatabaseTools InitTools(DbAccess db);
    }
}