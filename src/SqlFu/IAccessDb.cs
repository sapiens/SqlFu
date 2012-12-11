using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CavemanTools.Model;
using SqlFu.DDL;

namespace SqlFu
{
    public interface IAccessDb:IDisposable
    {
        IDatabaseTools DatabaseTools { get; }
        DbConnection Connection { get; }
        bool KeepAlive { get; set; }
        IHaveDbProvider Provider { get; }
        Action<DbCommand> OnCommand { get; set; }
        Action<IAccessDb> OnCloseConnection { get; set; }
        Action<IAccessDb> OnOpenConnection { get; set; }
        Action<ISqlStatement, Exception> OnException { get; set; }
        Action<IAccessDb> OnBeginTransaction { get; set; }
        Action<IAccessDb, bool> OnEndTransaction { get; set; }
        int TransactionDepth { get; }
        void CloseConnection(bool forceClose = false);
        DbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);
        int ExecuteCommand(string sql, params object[] args);

        [Obsolete("Use GetValue method")]
        T ExecuteScalar<T>(string sql, params object[] args);
        /// <summary>
        /// Returns the first column of the first row in the result set.
        /// All the other rows and columns are ignored
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        T GetValue<T>(string sql, params object[] args);
        T Get<T>(object id,string additionalPredicate=null,params object[] args);
        ResultSet<T> PagedQuery<T>(long skip,int take,string sql, params object[] args);
        IEnumerable<T> Query<T>(string sql, params object[] args);
        List<T> Fetch<T>(string sql, params object[] args);

        /// <summary>
        /// Prepares sql statement
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        ISqlStatement WithSql(string sql, params object[] args);

        IPagedSqlStatement WithSql(long skip,int take,string sql, params object[] args);

        /// <summary>
        /// Executes sproc
        /// </summary>
        /// <param name="sprocName"></param>
        /// <param name="arguments">Arguments as an anonymous object, output parameters names must be prefixed with _ </param>
        /// <example>
        /// ExecuteStoredProcedure("sprocName",new{Id=1,_OutValue=""})
        /// </example>
        /// <returns></returns>
        StoredProcedureResult ExecuteStoredProcedure(string sprocName, object arguments=null);
    }
}