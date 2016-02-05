using System;
using System.Data.Common;

namespace SqlFu.Providers
{
    public interface IDbProvider : IEscapeIdentifier, IDbProviderExceptions, IDbProviderHelpers
    {
        void SetupParameter(DbParameter dbDataParameter, string key, object value);
        /// <summary>
        /// Replaces @ with provider parameter prefix
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        string FormatParameters(string sql, string[] paramNames);

        /// <summary>
        /// Provider gets to make final changes to command before it executes
        /// </summary>
        /// <param name="cmd"></param>
        void OnCommandExecuting(DbCommand cmd);
        string ProviderId { get; }
        DbConnection CreateConnection();
        string GetColumnType(Type type);
       
        IDatabaseTools DatabaseTools { get; }

        /// <summary>
        /// Formats table index create options
        /// </summary>
        /// <param name="idxDef"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        string FormatIndexOptions(string idxDef,string options="");
        
        /// <summary>
        /// For create table statements
        /// </summary>
        /// <returns></returns>
        string GetIdentityKeyword();


        IDbProviderExpressions ExpressionsHelper { get; }
    }
}