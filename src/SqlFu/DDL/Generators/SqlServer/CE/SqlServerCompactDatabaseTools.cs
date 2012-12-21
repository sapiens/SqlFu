namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactDatabaseTools : SqlServerDatabaseTools
    {
        public SqlServerCompactDatabaseTools(DbAccess db) : base(db)
        {
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            //return new SqlServerCompactDDLWriter(Db);
            return null;
        }
    }
}