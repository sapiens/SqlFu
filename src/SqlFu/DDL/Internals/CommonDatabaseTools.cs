using System;
using SqlFu.DDL.Generators;

namespace SqlFu.DDL.Internals
{
    internal abstract class CommonDatabaseTools : IDatabaseTools
    {
        protected readonly SqlFuConnection Db;

        public CommonDatabaseTools(SqlFuConnection db)
        {
            Db = db;
        }

        protected abstract string FormatName(string s);

        public abstract bool ConstraintExists(string name, string schema = null);
        public abstract bool IndexExists(string name, string table, string schema = null);

        public abstract bool TableHasColumn(string table, string column, string schema = null);
        public abstract bool TableHasPrimaryKey(string table, string schema = null);
        public abstract string GetPrimaryKeyName(string tableName, string schema = null);

        public virtual void DropTable(string tableName)
        {
            tableName.MustNotBeEmpty();
            var cmd = string.Format("drop table {0}", FormatName(tableName));
            Db.ExecuteCommand(cmd);
        }

        public abstract bool TableExists(string name, string schema);


        public abstract void RenameTable(string oldName, string newName);


        public void TruncateTable(string name)
        {
            name.MustNotBeEmpty();
            Db.ExecuteCommand(string.Format("truncate table {0}", FormatName(name)));
        }

        public ICreateTable GetCreateTableBuilder(string name, IfTableExists option = IfTableExists.Throw)
        {
            return new CreateTableBuilder(Db, GetDDLWriter(), name, option);
        }

        /// <summary>
        /// Creates a fluent builder object to help you generate a Create Table script, starting from an existing type.
        /// You can customize it further
        /// </summary>
        /// <returns></returns>
        public ICreateTable GetCreateTableBuilder<T>()
        {
            var schema = new TableSchema(typeof (T));
            return new CreateTableBuilder(Db, GetDDLWriter(), schema);
        }

        protected abstract IGenerateDDL GetDDLWriter();

        public IModifyTable GetAlterTableBuilder(string name)
        {
            return new ModifyTableBuilder(Db, GetDDLWriter(), name);
        }
    }
}