using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class CommonDDLWriter : IGenerateDDL
    {
        protected readonly SqlFuConnection Db;
        private TableSchema _table;

        public CommonDDLWriter(SqlFuConnection db, DbEngine engine)
        {
            db.MustNotBeNull();
            Db = db;
            Builder = new StringBuilder();
            _engine = engine;
        }

        protected StringBuilder Builder { get; private set; }

        protected TableSchema Table
        {
            get { return _table; }
        }

        public virtual string GenerateCreateTable(TableSchema table)
        {
            _table = table;
            ColonBeforeConstraints = true;
            table.Options.Use(_engine);
            Builder.Clear();
            Builder.Append("create table ");
            WriteTableName();
            Builder.Append(" (");
            WriteColumns(table.Columns);
            WriteConstraints(table.Constraints);
            Builder.Append(")");
            WriteEndTableOptions();
            WriteIndexes(table.Indexes);
            return Builder.ToString();
        }

        protected void WriteColumns(ColumnsCollection columns)
        {
            var writer = GetColumnWriter();
            foreach (var col in columns)
            {
                Builder.AppendLine();
                writer.Write(col);
                Builder.Append(",");
            }
            Builder.RemoveLast();
        }

        protected abstract void WriteTableName();


        protected virtual void WriteEndTableOptions()
        {
            Builder.Append(";");
        }

        protected abstract AbstractColumnWriter GetColumnWriter();

        protected abstract AbstractUniqueKeyWriter GetUniqueKeyWriter();
        protected abstract AbstractCheckWriter GetCheckWriter();
        protected abstract AbstractForeignKeyWriter GetForeignKeyWriter();

        protected abstract AbstractIndexWriter GetIndexWriter();

        protected bool ColonBeforeConstraints = false;

        protected void WriteConstraints(ConstraintsCollection constraints, bool isAdding = false)
        {
            var uniq = GetUniqueKeyWriter();
            if (ColonBeforeConstraints)
            {
                Builder.AppendLine(",");
            }
            if (constraints.PrimaryKey != null && !constraints.PrimaryKey.Ignore)
            {
                uniq.Write(constraints.PrimaryKey);
                Builder.AppendLine(",");
                if (isAdding) Builder.AppendFormat(" {0} ", GetAddConstraintPrefix());
            }

            if (constraints.Uniques.Count > 0)
            {
                foreach (var uc in constraints.Uniques)
                {
                    uniq.Write(uc);
                    Builder.AppendLine(",");
                    if (isAdding) Builder.AppendFormat(" {0} ", GetAddConstraintPrefix());
                }
            }

            if (constraints.ForeignKeys.Count > 0)
            {
                var w = GetForeignKeyWriter();
                foreach (var key in constraints.ForeignKeys)
                {
                    w.Write(key);
                    Builder.AppendLine(",");
                    if (isAdding) Builder.AppendFormat(" {0} ", GetAddConstraintPrefix());
                }
            }

            if (constraints.Checks.Count > 0)
            {
                var chkWriter = GetCheckWriter();
                foreach (var ch in constraints.Checks)
                {
                    chkWriter.Write(ch);
                    Builder.AppendLine(",");
                    if (isAdding) Builder.AppendFormat(" {0} ", GetAddConstraintPrefix());
                }
            }
            if (isAdding) Builder.RemoveLastIfEquals(" " + GetAddConstraintPrefix() + " ");
            var custom = constraints.GetSpecificConstraints(_engine);
            if (!custom.IsNullOrEmpty())
            {
                foreach (var ch in custom)
                {
                    Builder.AppendLine().Append(ch).AppendLine(",");
                }
            }
            Builder.RemoveLastIfEquals(",\r\n");
        }

        protected virtual string GetAddConstraintPrefix()
        {
            return "add";
        }

        private readonly DbEngine _engine;

        protected void WriteIndexes(IndexCollection indexes)
        {
            if (indexes.Count > 0)
            {
                var w = GetIndexWriter();
                foreach (var idx in indexes)
                {
                    Builder.AppendLine();
                    w.Write(idx);
                    Builder.Append(";");
                }
            }

            var custom = indexes.GetSpecificIndexes(_engine);
            if (!custom.IsNullOrEmpty())
            {
                foreach (var ch in custom)
                {
                    Builder.AppendLine(ch + ";");
                }
            }
        }


        public static void WriteColumnsNames(string columns, StringBuilder builder, Func<string, string> formatter)
        {
            columns.MustNotBeEmpty();
            formatter.MustNotBeNull();
            WriteColumnsNames(columns.Split(','), builder, formatter);
        }

        public static void WriteColumnsNames(ICollection<string> columns, StringBuilder builder,
                                             Func<string, string> formatter)
        {
            columns.MustNotBeNull();
            formatter.MustNotBeNull();
            foreach (var column in columns)
            {
                builder.Append(formatter(column.Trim())).Append(",");
            }
            builder.RemoveLast();
        }

        /// <summary>
        /// Escapes columns names using the specified formatter
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="formatter"></param>
        /// <returns></returns>
        public static string GetEscapedNames(string columns, Func<string, string> formatter)
        {
            columns.MustNotBeEmpty();
            return string.Join(",", columns.Split(',').Select(c => formatter(c.Trim())));
        }

        public virtual string GenerateAlterTable(TableSchema schema)
        {
            _table = schema;
            Builder.Clear();
            DefineExistingColumns();
            WriteColumnRenames();
            WriteDroppedIndexes();
            WriteDroppedConstraints();
            GetChangedColumnsManager().Write(schema.ModifiedColumns);
            WriteDroppedColumns();
            WriteColumnsAdditions();
            WriteConstraintsAdditions();
            WriteIndexes(Table.Indexes);

            return Builder.ToString();
        }

        protected virtual void DefineExistingColumns()
        {
        }

        protected abstract AbstractChangedColumnsManager GetChangedColumnsManager();

        private void WriteColumnRenames()
        {
            if (Table.ModifiedColumns.Renames.Count == 0) return;

            foreach (var col in Table.ModifiedColumns.Renames)
            {
                WriteRenameColumn(col);
            }
        }

        protected abstract void WriteRenameColumn(ColumnModifications col);


        private void WriteDroppedIndexes()
        {
            if (Table.Indexes.Dropped.Count > 0)
            {
                var writer = GetDropIndexWriter();
                foreach (var idx in Table.Indexes.Dropped)
                {
                    writer.Write(idx);
                }
            }
        }

        protected abstract AbstractDropIndexWriter GetDropIndexWriter();

        private void WriteDroppedConstraints()
        {
            if (Table.Constraints.Dropped.Count > 0)
            {
                var writer = GetDropConstraintWriter();
                foreach (var cn in Table.Constraints.Dropped)
                {
                    writer.Write(cn);
                }
            }
        }

        protected abstract AbstractDropConstraintWriter GetDropConstraintWriter();

        private void WriteDroppedColumns()
        {
            if (Table.ModifiedColumns.DroppedColumns.Count > 0)
            {
                var writer = GetDropColumnWriter();
                foreach (var cn in Table.ModifiedColumns.DroppedColumns)
                {
                    writer.Write(cn);
                }
            }
        }

        protected abstract AbstractDropColumnWriter GetDropColumnWriter();

        private void WriteColumnsAdditions()
        {
            if (Table.Columns.Count > 0)
            {
                var writer = GetColumnWriter();
                var table = Escape(Table.Name);
                foreach (var col in Table.Columns)
                {
                    Builder.AppendFormat("alter table {0} add ", table);
                    writer.Write(col);
                    Builder.Append(";\n");
                }
            }
        }

        protected abstract string Escape(string name);

        private void WriteConstraintsAdditions()
        {
            if (Table.Constraints.HasConstraints)
            {
                var table = Escape(Table.Name);
                Builder.AppendFormat("alter table {0} add", Escape(Table.Name));
                WriteConstraints(Table.Constraints, true);
                Builder.Append(";\n");
            }
        }
    }

    internal interface IGenerateDDL
    {
        string GenerateCreateTable(TableSchema table);
        string GenerateAlterTable(TableSchema schema);
    }
}