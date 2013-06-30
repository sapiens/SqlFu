using System;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresDatabaseTools : CommonDatabaseTools
    {
        public PostgresDatabaseTools(SqlFuConnection db) : base(db)
        {
        }

        protected override string FormatName(string s)
        {
            return PostgresProvider.EscapeIdentifier(s);
        }

        public override bool ConstraintExists(string name, string schema = null)
        {
            name.MustNotBeEmpty();
            if (schema.IsNullOrEmpty()) schema = "public";
            return
                Db.GetValue<bool>(
                    @"select count(*) from information_schema.table_constraints where constraint_name=@0 and constraint_schema=@1",
                    name, schema);
        }

        public override bool IndexExists(string name, string table, string schema = null)
        {
            name.MustNotBeEmpty();
            table.MustNotBeEmpty();
            return Db.GetValue<bool>(@"SELECT count(*) from pg_catalog.pg_indexes
where tablename=@0 and indexname=@1", table, name);
        }

        public override bool TableHasColumn(string table, string column, string schema = null)
        {
            table.MustNotBeEmpty();
            column.MustNotBeEmpty();
            return Db.GetValue<bool>(@"SELECT count(*) from information_schema.columns
where table_name=@0 and column_name=@1", table, column);
        }

        public override bool TableHasPrimaryKey(string table, string schema = null)
        {
            return !GetPrimaryKeyName(table, schema).IsNullOrEmpty();
        }

        public override string GetPrimaryKeyName(string tableName, string schema = null)
        {
            tableName.MustNotBeEmpty();
            if (schema.IsNullOrEmpty()) schema = "public";
            return
                Db.GetValue<string>(
                    @"select constraint_name from information_schema.table_constraints where table_schema=@0 and table_name=@1 and constraint_type='PRIMARY KEY'",
                    schema, tableName);
        }

        public override bool TableExists(string name, string schema)
        {
            name.MustNotBeEmpty();
            if (schema.IsNullOrEmpty()) schema = "public";
            return
                Db.GetValue<bool>(
                    @"select count(*) from information_schema.tables where table_schema=@0 and table_name=@1", schema,
                    name);
        }

        public override void RenameTable(string oldName, string newName)
        {
            oldName.MustNotBeEmpty();
            newName.MustNotBeEmpty();
            Db.ExecuteCommand(string.Format("alter table {0} rename to {1}", PostgresProvider.EscapeIdentifier(oldName),
                                            PostgresProvider.EscapeIdentifier(newName)));
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            return new PostgresqlDDLWriter(Db);
        }
    }
}