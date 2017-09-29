using System;
using System.Data.Common;
using SqlFu.Providers;

namespace SqlFu
{
    public static class InitStorage
    {
        public static void CreateStorage(this DbConnection cnx,Func<IEscapeIdentifier,string> getSql,Action<SqlFuConfig> config=null)
        {
            config?.Invoke(cnx.SqlFuConfig());
            cnx.AddDbObjectOrIgnore(getSql(cnx.Provider()));
        }
    }
}