using System;
using System.Collections.Generic;
using System.Data.Common;
using SqlFu.Providers;

namespace SqlFu
{
    public static class InitStorage
    {
      
        public static void CreateStorage(this DbConnection cnx,Action<ICreateForProviders> provSql,Action<SqlFuConfig> config=null)
        {
            var reg=new SqlForProviders();
            provSql(reg);
            var sql = reg.GetSqlForProvider(cnx.Provider());
            config?.Invoke(cnx.SqlFuConfig());
            cnx.AddDbObjectOrIgnore(sql);
        }
    }

    public interface ICreateForProviders
    {
        ICreateForProviders When<T>(Func<IEscapeIdentifier,string> getSql) where T : IDbProvider;
    }

    public class SqlForProviders : Dictionary<Type, Func<IEscapeIdentifier, string>>, ICreateForProviders
    {
        public string GetSqlForProvider(IDbProvider prov)
        {
            var func = this.GetValueOrDefault(prov.GetType());
            if(func==null) throw new InvalidOperationException($"No sql is specified for provider {prov.ProviderId}");
            return func(prov);
        }

        public ICreateForProviders When<T>(Func<IEscapeIdentifier, string> getSql) where T : IDbProvider
        {
            getSql.MustNotBeNull();
            this[typeof(T)] = getSql;
            return this;
        }
    }
}