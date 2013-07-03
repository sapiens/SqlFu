using System;
using System.Collections.Generic;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerDatabaseTools : CommonDatabaseTools
    {
        public SqlServerDatabaseTools(SqlFuConnection db) : base(db)
        {
        }

        protected override string FormatName(string s)
        {
            return SqlServerProvider.EscapeIdentifier(s);
        }

        public override bool ConstraintExists(string name, string schema = null)
        {
            name.MustNotBeEmpty();
            return
                Db.GetValue<bool>(
                    @"SELECT
    count(*)
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
    WHERE CONSTRAINT_NAME =@0 and constraint_schema=@1", name, schema ?? "dbo");
        }

        public override bool IndexExists(string name, string table, string schema = null)
        {
            name.MustNotBeEmpty();
            return
                Db.GetValue<bool>(
                    @"SELECT
    count(*)
    FROM sys.indexes
    WHERE name=@0", name);
        }

        public override bool TableHasColumn(string table, string column, string schema = null)
        {
            table.MustNotBeEmpty();
            column.MustNotBeEmpty();
            return
                Db.GetValue<bool>(
                    @"select count(*) from information_schema.columns where table_schema=@0 and table_name=@1 and column_name=@2",
                    schema ?? "dbo", table, column);
        }

        public override bool TableHasPrimaryKey(string table, string schema = null)
        {
            table.MustNotBeEmpty();
            return Db.GetValue<bool>(@"
SELECT  count(*)
FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
where  i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID)=@0
", table);
        }

        public override string GetPrimaryKeyName(string tableName, string schema = null)
        {
            tableName.MustNotBeEmpty();
            return Db.GetValue<string>(@"
SELECT  i.name AS IndexName        
FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
where  i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID)=@0
", tableName);
        }

        #region Maybe useful sql

        /*
         * 
         * SELECT  count(*)i.name AS IndexName,
        OBJECT_NAME(ic.OBJECT_ID) AS TableName,
        COL_NAME(ic.OBJECT_ID,ic.column_id) AS ColumnName
FROM    sys.indexes AS i INNER JOIN 
        sys.index_columns AS ic ON  i.OBJECT_ID = ic.OBJECT_ID
                                AND i.index_id = ic.index_id
where  i.is_primary_key = 1 and OBJECT_NAME(ic.OBJECT_ID)='Shops'
         */

        #endregion

        public override void DropTable(string tableName)
        {
            if (TableExists(tableName)) base.DropTable(tableName);
        }


        public override bool TableExists(string name, string schema = null)
        {
            var sql =
                @"SELECT 1 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = @0";
            var param = new List<object>();
            param.Add(name);
            if (!schema.IsNullOrEmpty())
            {
                sql += " and TABLE_SCHEMA= @1";
                param.Add(schema);
            }
            return Db.GetValue<bool?>(sql, param.ToArray()).HasValue;
        }

        public override void RenameTable(string oldName, string newName)
        {
            oldName.MustNotBeEmpty();
            newName.MustNotBeEmpty();
            var sql = string.Format("exec sp_rename '{0}','{1}'", oldName, newName);
            Db.ExecuteCommand(sql);
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            return new SqlServerDDLWriter(Db);
        }
    }
}