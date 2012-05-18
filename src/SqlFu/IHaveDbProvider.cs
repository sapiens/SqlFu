using System.Data;

namespace SqlFu
{
    public interface IHaveDbProvider
    {
        string EscapeName(string s);
        LastInsertId ExecuteInsert(SqlStatement sql, string idKey);
        IDbConnection CreateConnection();
        string ParamPrefix { get; }
        void MakePaged(string sql, out string selecSql, out string countSql);

        void SetupParameter(IDbDataParameter param, string name, object value);
        string FormatSql(string sql, params string[] paramNames);
        void OnCommandExecuting(IDbCommand cmd);
    }
}