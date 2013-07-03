using System;
using System.Linq;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteDatabaseTools : CommonDatabaseTools
    {
        public SqliteDatabaseTools(SqlFuConnection db)
            : base(db)
        {
        }

        protected override string FormatName(string s)
        {
            return SqliteProvider.EscapeIdentifier(s);
        }

        public override bool ConstraintExists(string name, string schema = null)
        {
            throw new NotSupportedException("Sqlite ignores names of constraints");
        }

        public override bool IndexExists(string name, string table, string schema = null)
        {
            name.MustNotBeEmpty();
            return Db.GetValue<bool>("SELECT count(name) FROM sqlite_master WHERE type='index' AND name=@0", name);
        }

        public override bool TableHasColumn(string table, string column, string schema = null)
        {
            table.MustNotBeEmpty();
            column.MustNotBeEmpty();
            return
                Db.Query<dynamic>(string.Format("pragma table_info('{0}')", table))
                  .Any(r => column.Equals(r.name.ToUpperInvariant(), StringComparison.InvariantCultureIgnoreCase));
        }

        public override bool TableHasPrimaryKey(string table, string schema = null)
        {
            table.MustNotBeEmpty();
            return Db.Query<dynamic>(string.Format("pragma table_info('{0}')", table)).Any(r => r.pk == 1);
        }

        public override string GetPrimaryKeyName(string tableName, string schema = null)
        {
            return null;
        }

        public override bool TableExists(string name, string schema)
        {
            name.MustNotBeEmpty();
            return Db.GetValue<bool>("SELECT count(name) FROM sqlite_master WHERE type='table' AND name=@0", name);
        }

        public override void RenameTable(string oldName, string newName)
        {
            oldName.MustNotBeEmpty();
            newName.MustNotBeEmpty();
            Db.ExecuteCommand(string.Format("alter table {0} rename to {1}", FormatName(oldName), FormatName(newName)));
        }

        protected override IGenerateDDL GetDDLWriter()
        {
            return new SqliteDDLWriter(Db);
        }
    }
}