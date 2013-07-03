namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactDDLWriter
    {
        //public SqlServerCompactDDLWriter(DbConnection db) : base(db)
        //{
        //}

        //public override string GenerateCreateTable(TableSchema table)
        //{
        //    Builder.Clear();
        //    Builder.AppendFormat("create table {0} (", SqlServerCEProvider.FormatName(table.Name));
        //    WriteNewColumns(table.Columns);
        //    WriteNewConstraints(table.Constraints);
        //    Builder.Append(");");
        //    WriteNewIndexes(table.Indexes);
        //    return Builder.ToString();
        //}

        //void WriteNewColumns(ColumnsCollection columns)
        //{
        //    var writer = new SqlServerCompactColumnWriter(Builder);
        //    foreach (var col in columns)
        //    {
        //        Builder.AppendLine();
        //        writer.Write(col);
        //        Builder.Append(",");
        //    }
        //    Builder.RemoveLast();
        //}

        //void WriteNewConstraints(ConstraintsCollection constraints)
        //{
        //    WritePrimaryKey(constraints.PrimaryKey);

        //    if (constraints.Uniques.Count > 0)
        //    {
        //        var w = new SqlServerCompactUniqueKeyWriter(Builder);
        //        foreach (var uc in constraints.Uniques)
        //        {
        //            Builder.AppendLine();
        //            w.Write(uc);
        //            Builder.Append(",");
        //        }

        //    }

        //    if (constraints.ForeignKeys.Count > 0)
        //    {
        //        var w = new SqlServerCompactForeignKeyWriter(Builder);
        //        foreach (var key in constraints.ForeignKeys)
        //        {
        //            Builder.AppendLine();
        //            w.Write(key);
        //            Builder.Append(",");
        //        }
        //    }

        //    Builder.RemoveLastIfEquals(',');
        //}

        //private void WritePrimaryKey(UniqueKeyConstraint primaryKey)
        //{
        //    if (primaryKey != null)
        //    {
        //        var writer = new SqlServerCompactUniqueKeyWriter(Builder);
        //        Builder.AppendLine();
        //        writer.Write(primaryKey);
        //        Builder.Append(",");
        //    }
        //}

        //private void WriteNewIndexes(IndexCollection indexes)
        //{
        //    if (indexes.Count > 0)
        //    {
        //        var w = new SqlServerIndexWriter(Builder);
        //        foreach (var idx in indexes)
        //        {
        //            Builder.AppendLine();
        //            w.Write(idx);
        //            Builder.Append(";");
        //        }
        //    }
        //}


        //public override string GenerateAlterTable(TableSchema table)
        //{
        //    Builder.Clear();

        //    WriteColumnRenames(table);

        //    WriteDroppedIndexes(table);

        //    WriteDroppedConstraints(table);

        //    var columns = new SqlServerCompactModifiedColumnsWriter(Builder, Db);
        //    columns.WriteColumnsChanges(table);

        //    WriteDroppedColumns(table);

        //    WriteColumnsAdditions(table);

        //    WriteConstraintsAdditions(table);

        //    WriteNewIndexes(table.Indexes);

        //    return Builder.ToString();
        //}

        //private void WriteDroppedIndexes(TableSchema table)
        //{
        //    if (table.DroppedIndexes.Count > 0)
        //    {
        //        foreach(var idx in table.DroppedIndexes)
        //        {
        //            Builder.AppendFormat("drop index {0}.{1};\n",table.Name,idx);
        //        }
        //    }
        //}

        //private void WriteColumnRenames(TableSchema table)
        //{
        //    if (table.ModifiedColumns.Renames.Count == 0) return;

        //    foreach (var col in table.ModifiedColumns.Renames)
        //    {
        //        Builder.AppendFormat(@"alter Table [{2}] add [{1}];update [{2}] set [{1}] = [{0}];alter Table [{2}] drop column [{0}];\n", col.Current.Name, col.NewName, table.Name);
        //    }
        //}

        //private void WriteDroppedConstraints(TableSchema table)
        //{
        //    if (table.DroppedConstraints.Count > 0)
        //    {
        //        foreach(var cn in table.DroppedConstraints)
        //        {
        //            Builder.AppendFormat("alter table {0} drop constraint {1};\n", SqlServerProvider.FormatName(table.Name),cn);
        //        }                
        //    }
        //}

        //private void WriteDroppedColumns(TableSchema table)
        //{
        //    if (table.ModifiedColumns.DroppedColumns.Count > 0)
        //    {
        //        foreach(var col in table.ModifiedColumns.DroppedColumns)
        //        {
        //            Builder.AppendFormat("alter table {0} drop column {1};\n", SqlServerProvider.FormatName(table.Name),col);
        //        }
        //    }
        //}

        //private void WriteColumnsAdditions(TableSchema table)
        //{
        //    if (table.Columns.Count > 0)
        //    {
        //        Builder.AppendFormat("alter table {0} add ", SqlServerProvider.FormatName(table.Name));
        //        WriteNewColumns(table.Columns);
        //        Builder.Append(";\n");
        //    }

        //}

        //private void WriteConstraintsAdditions(TableSchema table)
        //{
        //    if (table.Constraints.HasConstraints)
        //    {
        //        Builder.AppendFormat("alter table {0} add ", SqlServerProvider.FormatName(table.Name));
        //        WriteNewConstraints(table.Constraints);
        //        Builder.Append(";\n");
        //    }
        //}
    }
}