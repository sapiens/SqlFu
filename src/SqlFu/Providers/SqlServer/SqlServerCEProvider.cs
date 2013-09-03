using System.Data.Common;
using SqlFu.DDL;
using SqlFu.DDL.Generators.SqlServer.CE;

namespace SqlFu.Providers.SqlServer
{
    public class SqlServerCEProvider : SqlServerProvider
    {
        public const string ProviderName = "System.Data.SqlServerCe.4.0";

        public SqlServerCEProvider() : base(ProviderName)
        {
        }


        public override LastInsertId ExecuteInsert(DbCommand cmd, string idKey)
        {
            cmd.Execute();
            cmd.Reset();
            cmd.CommandText = "select @@IDENTITY as id";
            var rez = cmd.GetRawValue();
            return new LastInsertId(rez);
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + body;
            selecSql = string.Format("{0} OFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", sql,
                                     PreparePagedStatement.SkipParameterName,
                                     PreparePagedStatement.TakeParameterName);
        }

        protected override IDatabaseTools InitTools(SqlFuConnection db)
        {
            return new SqlServerCompactDatabaseTools(db);
        }

        public override DbEngine ProviderType
        {
            get { return DbEngine.SqlServerCE; }
        }

        public override IDbProviderExpressionHelper BuilderHelper
        {
            get { return new SqlServerCEBuilderHelper(); }
        }
    }
}