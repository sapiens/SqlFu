﻿using System;
using System.Data.Common;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers
{
    public interface IDbProvider : IEscapeIdentifier
    {
        DbFunctions Functions { get; }

        void SetupParameter(DbParameter dbDataParameter, string key, object value);
        /// <summary>
        /// Replaces @ with provider parameter prefix
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <returns></returns>
        string FormatSql(string sql, string[] paramNames);

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
        /// For genrate create table statements
        /// </summary>
        /// <returns></returns>
        string GetIdentityKeyword();
        

        bool IsDbBusy(DbException ex);

        bool IsUniqueViolation(DbException ex, string keyName = "");

        string GetSqlForDropTableIfExists(string name, string schema = null);

        /// <summary>
        /// Adds sql to return newly inserted id
        /// </summary>
        /// <param name="values">String containing the values to insert e.g: values (@0,@1)</param>
        /// <param name="identityColumn"></param>
        /// <returns></returns>
        string AddReturnInsertValue(string values, string identityColumn);
        
        IDbProviderExpressions GetExpressionsHelper();
        string FormatQueryPagination(string sql, Pagination page, ParametersManager pm);
     
    }
}