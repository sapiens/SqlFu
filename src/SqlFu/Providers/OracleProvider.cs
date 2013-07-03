namespace SqlFu.Providers
{
    //public class OracleProvider : AbstractProvider
    //{
    //    public const string ProviderName = "Oracle.DataAccess.Client";

    //    public OracleProvider(string provider = ProviderName)
    //        : base(provider ?? ProviderName)
    //    {
    //    }

    //    public override void OnCommandExecuting(IDbCommand cmd)
    //    {
    //        if (cmd.CommandType == CommandType.Text)
    //        {
    //            dynamic ocmd = cmd;
    //            ocmd.BindByName = true;
    //        }
    //    }

    //    public static string FormatName(string s)
    //    {
    //        return "\"" + s + "\"";
    //    }

    //    public override DbEngine ProviderType
    //    {
    //        get { return DbEngine.Oracle; }
    //    }

    //    /// <summary>
    //    /// Gets expression query builder helpers for the rdbms.
    //    /// Internal usage
    //    /// </summary>
    //    public override IDbProviderExpressionHelper BuilderHelper
    //    {
    //        get { throw new NotImplementedException("Not supported");
    //            }
    //    }

    //    protected override IDatabaseTools InitTools(DbAccess db)
    //    {
    //        throw new NotImplementedException("Oracle is not supported yet");
    //    }

    //    public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
    //    {
    //        IDbDataParameter param = null;
    //        if (!string.IsNullOrEmpty(idKey))
    //        {
    //            sql.Sql += string.Format(" returning {0} into :newid", EscapeName(idKey));
    //            var cmd = sql.Command;
    //            param = cmd.CreateParameter();
    //            param.ParameterName = ":newid";
    //            param.Value = DBNull.Value;
    //            param.Direction = ParameterDirection.ReturnValue;
    //            param.DbType = DbType.Int64;
    //            cmd.Parameters.Add(param);
    //        }
    //        using (sql)
    //        {
    //            sql.Execute();
    //            if (param == null) return LastInsertId.Empty;
    //            return new LastInsertId(param.Value);
    //        }
    //    }

    //    public override void MakePaged(string sql, out string selecSql, out string countSql)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string ParamPrefix
    //    {
    //        get { return ":"; }
    //    }
    //}
}