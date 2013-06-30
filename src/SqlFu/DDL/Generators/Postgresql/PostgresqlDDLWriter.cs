using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresqlDDLWriter : CommonDDLWriter
    {
        public PostgresqlDDLWriter(SqlFuConnection db)
            : base(db, DbEngine.PostgreSQL)
        {
        }


        protected override void WriteTableName()
        {
            Builder.Append(Escape(Table.Name));
        }

        protected override AbstractColumnWriter GetColumnWriter()
        {
            return new PostgresColumnWriter(Builder);
        }

        protected override AbstractUniqueKeyWriter GetUniqueKeyWriter()
        {
            return new PostgresqlUniqueKeyWriter(Builder);
        }

        protected override AbstractCheckWriter GetCheckWriter()
        {
            return new PostgresCheckWriter(Builder);
        }

        protected override AbstractForeignKeyWriter GetForeignKeyWriter()
        {
            return new PostgresForeignKeyWriter(Builder);
        }

        protected override AbstractIndexWriter GetIndexWriter()
        {
            return new PostgresIndexWriter(Builder);
        }

        //--------------------
        protected override AbstractChangedColumnsManager GetChangedColumnsManager()
        {
            return new PostgresChangedColumnsManager(Builder);
        }

        protected override void WriteRenameColumn(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} rename {1} to {2};\n\r", Escape(col.TableName),
                                 Escape(col.Current.Name), Escape(col.NewName));
        }

        protected override AbstractDropIndexWriter GetDropIndexWriter()
        {
            return new PostgresDropIndexWriter(Builder);
        }

        protected override AbstractDropConstraintWriter GetDropConstraintWriter()
        {
            return new PostgresDropConstraintWriter(Builder, Db.DatabaseTools);
        }

        protected override AbstractDropColumnWriter GetDropColumnWriter()
        {
            return new PostgresDropColumnWriter(Builder);
        }

        protected override string Escape(string name)
        {
            return PostgresProvider.EscapeIdentifier(name);
        }

        public static void WriteColumnsNames(string columns, StringBuilder builder)
        {
            WriteColumnsNames(columns, builder, PostgresProvider.EscapeIdentifier);
        }
    }
}