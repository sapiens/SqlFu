using System;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteDDLWriter : CommonDDLWriter
    {
        public SqliteDDLWriter(SqlFuConnection db)
            : base(db, DbEngine.SQLite)
        {
            ColonBeforeConstraints = true;
        }

        public override string GenerateCreateTable(TableSchema table)
        {
            if (table.Columns.IsIdentityDefined)
            {
                var pk = table.Constraints.PrimaryKey;
                if (pk != null) pk.Ignore = true;
            }
            return base.GenerateCreateTable(table);
        }

        public override string GenerateAlterTable(TableSchema schema)
        {
            throw new NotSupportedException("Sqlite alter table is not supported");
        }

        protected override void WriteTableName()
        {
            if (Table.CreationOption == IfTableExists.Ignore)
            {
                Builder.Append("if not exists ");
            }
            Builder.Append(Escape(Table.Name));
        }

        protected override AbstractColumnWriter GetColumnWriter()
        {
            return new SqliteColumnWriter(Builder);
        }

        protected override AbstractUniqueKeyWriter GetUniqueKeyWriter()
        {
            return new SqliteUniqueKeyWriter(Builder);
        }

        protected override AbstractCheckWriter GetCheckWriter()
        {
            return new SqliteCheckWriter(Builder);
        }

        protected override AbstractForeignKeyWriter GetForeignKeyWriter()
        {
            return new SqliteForeignKeyWriter(Builder);
        }

        protected override AbstractIndexWriter GetIndexWriter()
        {
            return new SqliteIndexWriter(Builder);
        }

        protected override AbstractChangedColumnsManager GetChangedColumnsManager()
        {
            return new SqliteChangedColumnsManager(Builder);
        }

        protected override void WriteRenameColumn(ColumnModifications col)
        {
            throw new NotImplementedException();
        }

        protected override AbstractDropIndexWriter GetDropIndexWriter()
        {
            throw new NotImplementedException();
        }

        protected override AbstractDropConstraintWriter GetDropConstraintWriter()
        {
            throw new NotImplementedException();
        }

        protected override AbstractDropColumnWriter GetDropColumnWriter()
        {
            return new SqliteDropColumnWriter(Builder);
        }

        protected override string Escape(string name)
        {
            return SqliteProvider.EscapeIdentifier(name);
        }
    }
}