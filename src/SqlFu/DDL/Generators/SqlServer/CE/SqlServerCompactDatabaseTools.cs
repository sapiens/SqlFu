using System;

namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactDatabaseTools : SqlServerDatabaseTools
    {
        public SqlServerCompactDatabaseTools(SqlFuConnection db) : base(db)
        {
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            throw new NotSupportedException();
        }
    }
}