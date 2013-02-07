using System.Data;
using System.Data.Common;
using SqlFu.DDL;

namespace SqlFu
{
    public interface IEscapeDbIdentifier
    {
        string EscapeName(string s);
    }

    public interface IHaveDbProvider : IEscapeDbIdentifier
    {
        LastInsertId ExecuteInsert(SqlStatement sql, string idKey);
        DbConnection CreateConnection();
        string ParamPrefix { get; }
        void MakePaged(string sql, out string selecSql, out string countSql);

        void SetupParameter(IDbDataParameter param, string name, object value);
        string FormatSql(string sql, params string[] paramNames);
        void OnCommandExecuting(IDbCommand cmd);
        DbEngine ProviderType { get; }
        IDatabaseTools GetTools(DbAccess db);
        /// <summary>
        /// Gets expression query builder helpers for the rdbms.
        /// Internal usage
        /// </summary>
        IDbProviderExpressionHelper BuilderHelper { get; }
    }
}