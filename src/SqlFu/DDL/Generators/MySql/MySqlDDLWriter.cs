using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlDDLWriter : CommonDDLWriter
    {
        public MySqlDDLWriter(SqlFuConnection db) : base(db, DbEngine.MySql)
        {
        }

        protected override void WriteTableName()
        {
            Builder.Append(Escape(Table.Name));
        }

        protected override void WriteEndTableOptions()
        {
            foreach (var optName in new[]
                {
                    MySqlTableOptions.Engine, MySqlTableOptions.AUTO_INCREMENT, MySqlTableOptions.Character_Set,
                    MySqlTableOptions.COLLATE
                })
            {
                var opt = Table.Options.Get(optName);
                if (opt != null)
                {
                    Builder.AppendLine(" " + opt);
                }
            }

            Builder.Append(";");
        }

        protected override void DefineExistingColumns()
        {
            //var cols = Table.ModifiedColumns.AllColumnsNames;
            var cols =
                Db.Query<dynamic>(@" select COLUMN_NAME as Name,COLUMN_TYPE as Type,COLLATION_NAME as Collation,CONCAT(COLUMN_NAME,' ',COLUMN_TYPE,' ',If(IS_NULLABLE=1,'null','not null'),if(COLUMN_DEFAULT is null,'',Concat(' default ',COLUMN_DEFAULT))) as Definition  from information_schema.`COLUMNS`
where TABLE_SCHEMA=@0 and TABLE_NAME=@1", Db.Connection.Database, Table.Name);
            /* select CONCAT(COLUMN_NAME,' ',COLUMN_TYPE,' ',If(IS_NULLABLE=1,'null','not null'),if(COLUMN_DEFAULT is null,'',Concat(' default ',COLUMN_DEFAULT)))  from information_schema.`COLUMNS`
where TABLE_SCHEMA='' and TABLE_NAME='' */
            foreach (var col in cols)
            {
                Table.ModifiedColumns[col.Name].Modifications.Current.Name = col.Name;
                Table.ModifiedColumns[col.Name].Modifications.Current.Type = col.Type;
                Table.ModifiedColumns[col.Name].Modifications.Current.Collation = col.Collation as string ?? "";
                Table.ModifiedColumns[col.Name].Modifications.Current.Definition = col.Definition;
            }
        }

        protected override AbstractColumnWriter GetColumnWriter()
        {
            return new MySqlColumnWriter(Builder);
        }

        protected override AbstractUniqueKeyWriter GetUniqueKeyWriter()
        {
            return new MySqlUniqueKeyWriter(Builder);
        }

        protected override AbstractCheckWriter GetCheckWriter()
        {
            return new MySqlCheckWriter(Builder);
        }

        protected override AbstractForeignKeyWriter GetForeignKeyWriter()
        {
            return new MySqlForeignKeyWriter(Builder);
        }

        protected override AbstractIndexWriter GetIndexWriter()
        {
            return new MySqlIndexWriter(Builder);
        }

        protected override AbstractChangedColumnsManager GetChangedColumnsManager()
        {
            return new MySqlChangedColumnManager(Builder);
        }

        protected override void WriteRenameColumn(ColumnModifications col)
        {
            Builder.AppendFormat("ALTER TABLE {0} CHANGE {1} {2}", Escape(col.TableName), Escape(col.Current.Name),
                                 col.Current.Definition.Replace(col.Current.Name, col.NewName));
            Builder.AppendLine(";");
        }

        protected override AbstractDropIndexWriter GetDropIndexWriter()
        {
            return new MySqlDropIndexWriter(Builder);
        }

        protected override AbstractDropConstraintWriter GetDropConstraintWriter()
        {
            return new MysqlDropConstraintWriter(Builder, Db);
        }

        protected override AbstractDropColumnWriter GetDropColumnWriter()
        {
            return new MySqlDropColumnWriter(Builder);
        }

        protected override string Escape(string name)
        {
            return MySqlProvider.EscapeIdentifier(name);
        }

        public static void WriteColumnsNames(string columns, StringBuilder builder)
        {
            WriteColumnsNames(columns, builder, MySqlProvider.EscapeIdentifier);
        }
    }
}