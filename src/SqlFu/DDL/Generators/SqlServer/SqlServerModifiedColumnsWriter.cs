namespace SqlFu.DDL.Generators.SqlServer
{
//    internal class SqlServerModifiedColumnsWriter
//    {
//        private readonly StringBuilder Builder;
//        private readonly DbConnection Db;

//        public SqlServerModifiedColumnsWriter(StringBuilder builder,DbConnection db)
//        {
//            Builder = builder;
//            Db = db;
//        }

//        public void WriteColumnsChanges(TableSchema table)
//        {
//            var mods = table.ModifiedColumns.ChangedColumns;
//            if (mods.Count == 0) return;
//            FillRealTableSchema(table);
//            var tableName = SqlServerProvider.EscapeIdentifier(table.Name);
//            var writer = new SqlServerColumnWriter(Builder);

//            foreach (var col in mods.Where(c =>c.IsDropped || c.DefaultDropped || c.Type != null || c.DefaultValue != null).Where(c => !string.IsNullOrEmpty(c.Current.DefaultConstraintName)))
//            {
//                Builder.AppendFormat("alter table {0} drop constraint {1};\n", tableName, col.Current.DefaultConstraintName);
//            }

//            foreach (var col in mods.Where(d=>!d.IsDropped))
//            {
//                Builder.AppendFormat("alter table {0} alter column {1} ", tableName, SqlServerProvider.EscapeIdentifier(col.Current.Name));
//                writer.Write(col);
//                Builder.AppendLine(";");
//            }

//            foreach (var cl in mods.Where(c => !string.IsNullOrEmpty(c.DefaultValue)))
//            {
//                Builder.AppendFormat("alter table {0} add constraint {1} default ('{2}') for {3};\n", tableName, string.Format("DF_{0}_{1}", table.Name, cl.CurrentName), cl.DefaultValue, cl.CurrentName);
//            }
//        }

//        void FillRealTableSchema(TableSchema table)
//        {
//            var columns = table.ModifiedColumns.AllColumnsNames;
//            // var tableName = SqlServerProvider.FormatName(table.Name);
//            var all = Db.Query<ColumnSchema>(
//                @"select COLUMN_NAME as ColumnName, DATA_TYPE as DataType
//, cast (CHARACTER_MAXIMUM_LENGTH as varchar(5))as CharacterMaximumLength, cast (NUMERIC_PRECISION as varchar(5)) as NumericPrecision, cast (NUMERIC_SCALE as varchar(5)) as NumericScale, COLLATION_NAME as CollationName from INFORMATION_SCHEMA.COLUMNS
//where TABLE_NAME = @0 and COLUMN_NAME in (@1)", table.Name, columns);
//            foreach (var cs in all)
//            {
//                var column = table.ModifiedColumns[cs.ColumnName];
//                column.Modifications.Current.Type = ExtractType(cs);
//                column.Modifications.Current.Collation = cs.CollationName;
//            }

//            var defaults = Db.Query<dynamic>(@"select c.name as colname, d.name as defname
//from sys.tables t
//    join
//    sys.default_constraints d
//        on d.parent_object_id = t.object_id
//    join
//    sys.columns c
//        on c.object_id = t.object_id
//        and c.column_id = d.parent_column_id
//where t.name = @0
//and c.name in (@1)", table.Name, columns);
//            foreach (var def in defaults)
//            {
//                var column = table.ModifiedColumns[def.colname];
//                column.Modifications.Current.DefaultConstraintName = def.defname;
//            }
//        }


//        string ExtractType(ColumnSchema schema)
//        {
//            var rez = schema.DataType;
//            if (!string.IsNullOrEmpty(schema.CharacterMaximumLength))
//            {
//                rez = rez + "(" + (schema.CharacterMaximumLength == "-1"
//                           ? "max"
//                           : schema.CharacterMaximumLength) + ")";
//                return rez;
//            }
//            if (!string.IsNullOrEmpty(schema.NumericPrecision))
//            {
//                rez = rez + "(" + schema.NumericPrecision;
//                if (!string.IsNullOrEmpty(schema.NumericScale))
//                {
//                    rez = rez + "," + schema.NumericScale;
//                }
//                return rez + ")";
//            }
//            return rez;
//        } 
//    }
}