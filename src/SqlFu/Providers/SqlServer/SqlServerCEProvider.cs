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


        public override LastInsertId ExecuteInsert(SqlStatement sql, string idKey)
        {
            //sql.Sql += ";Select @@IDENTITY as id";
            using (sql)
            {
                sql.Execute();
                using (var idquery = new SqlStatement(sql.Db))
                {
                    idquery.SetSql("select @@IDENTITY as id");
                    var rez = idquery.ExecuteScalar();
                    return new LastInsertId(rez);
                }
                
            }
        }

        public override void MakePaged(string sql, out string selecSql, out string countSql)
        {
            int formidx;
            var body = GetPagingBody(sql, out formidx);
            countSql = "select count(*) " + body;
            selecSql = string.Format("{0} OFFSET @{1} ROWS FETCH NEXT @{2} ROWS ONLY", sql,
                                     PagedSqlStatement.SkipParameterName,
                                     PagedSqlStatement.TakeParameterName);
        }

        protected override IDatabaseTools InitTools(DbAccess db)
        {
            return new SqlServerCompactDatabaseTools(db);
        }

        public override DbEngine ProviderType
        {
            get { return DbEngine.SqlServerCE; }
        }

        public override IDbProviderExpressionHelper BuilderHelper
        {
            get
            {
                return new SqlServerCEBuilderHelper();
            }
        }
    }
}