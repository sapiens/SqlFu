namespace SqlFu.DDL.Generators.SqlServer.CE
{
    class SqlServerCompactDatabaseTools : SqlServerDatabaseTools
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