using System.Data.Common;

namespace SqlFu.Providers
{
    /// <summary>
    /// Implementation should be thread safe. It will be used as a singleton
    /// </summary>
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

        IDbProviderExpressions ExpressionsHelper { get; }
        void ReplaceExpressionsProvider(IDbProviderExpressions prov);
		bool ObjectExists(DbException ex, string name = null);
	}
}