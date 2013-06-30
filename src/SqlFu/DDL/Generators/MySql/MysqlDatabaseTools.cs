using System;
using System.Linq;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MysqlDatabaseTools : CommonDatabaseTools
    {
        public MysqlDatabaseTools(SqlFuConnection db) : base(db)
        {
        }

        protected override string FormatName(string s)
        {
            return MySqlProvider.EscapeIdentifier(s);
        }

        public override bool TableHasPrimaryKey(string name, string schema = null)
        {
            name.MustNotBeEmpty();
            return Db.GetValue<bool>(@"
select count(*)
from information_schema.table_constraints
where constraint_schema = @0 and table_name=@1 and constraint_name='PRIMARY'
", Db.Connection.Database, name);
        }

        public override string GetPrimaryKeyName(string tableName, string schema = null)
        {
            if (TableHasPrimaryKey(tableName)) return "PRIMARY";
            return null;
        }

        public override void DropTable(string tableName)
        {
            tableName.MustNotBeEmpty();
            Db.ExecuteCommand(string.Format("drop table if exists `{0}`", tableName));
        }

        public override bool ConstraintExists(string name, string schema = null)
        {
            name.MustNotBeEmpty();
            return Db.GetValue<bool>(@"
select count(*)
from information_schema.table_constraints
where constraint_schema = @0 and constraint_name=@1
", Db.Connection.Database, name);
        }

        public override bool IndexExists(string name, string table, string schema = null)
        {
            name.MustNotBeEmpty();
            table.MustNotBeEmpty();
            var indexes = Db.Query<dynamic>(@"show index in " + FormatName(table) + " where Key_name='" + name + "'");
            return indexes.Any();
        }

        public override bool TableHasColumn(string table, string column, string schema = null)
        {
            table.MustNotBeEmpty();
            column.MustNotBeEmpty();
            return Db.GetValue<bool>(@"
SELECT count(*) FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @0 AND COLUMN_NAME = @1
", table, column);
        }

        public override bool TableExists(string name, string schema)
        {
            name.MustNotBeEmpty();
            if (!schema.IsNullOrEmpty())
            {
                name = schema + "." + name;
            }
            return Db.GetValue<bool>(@"Select count(*) from information_schema.tables
    WHERE table_schema = @0
    AND table_name = @1", Db.Connection.Database, name);
        }

        public override void RenameTable(string oldName, string newName)
        {
            oldName.MustNotBeEmpty();
            newName.MustNotBeEmpty();
            Db.ExecuteCommand(string.Format("rename table {0} to {1}", FormatName(oldName), FormatName(newName)));
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            return new MySqlDDLWriter(Db);
        }
    }
}