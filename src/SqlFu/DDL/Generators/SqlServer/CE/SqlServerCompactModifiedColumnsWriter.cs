using System.Linq;
using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactModifiedColumnsWriter
    {
        private readonly StringBuilder Builder;
        private readonly SqlFuConnection Db;

        public SqlServerCompactModifiedColumnsWriter(StringBuilder builder, SqlFuConnection db)
        {
            Builder = builder;
            Db = db;
        }

        public void WriteColumnsChanges(TableSchema table)
        {
            var mods = table.ModifiedColumns.ChangedColumns;
            if (mods.Count == 0) return;
            FillRealTableSchema(table);
            var writer = new SqlServerCompactColumnWriter(Builder);

            foreach (var col in mods.Where(c => c.DefaultDropped))
            {
                Builder.AppendFormat("alter table {0} alter column {1} drop default;\n", table.Name, col.Name);
            }

            foreach (var col in mods.Where(d => !d.IsDropped))
            {
                Builder.AppendFormat("alter table [{0}] alter column {1} ", table.Name,
                                     SqlServerProvider.EscapeIdentifier(col.Current.Name));
                writer.Write(col);
                Builder.AppendLine(";");
            }

            foreach (var cl in mods.Where(c => !string.IsNullOrEmpty(c.DefaultValue)))
            {
                Builder.AppendFormat("alter table {0} alter column {1} set default '{2}';\n", table.Name, cl.Name,
                                     cl.DefaultValue);
            }
        }

        private void FillRealTableSchema(TableSchema table)
        {
            var columns = table.ModifiedColumns.AllColumnsNames;
            // var tableName = SqlServerProvider.FormatName(table.Name);
            var all = Db.Query<ColumnSchema>(
                @"select COLUMN_NAME as ColumnName, DATA_TYPE as DataType
, cast (CHARACTER_MAXIMUM_LENGTH as varchar(5))as CharacterMaximumLength, cast (NUMERIC_PRECISION as varchar(5)) as NumericPrecision, cast (NUMERIC_SCALE as varchar(5)) as NumericScale, COLLATION_NAME as CollationName from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @0 and COLUMN_NAME in (@1)", table.Name, columns);
            foreach (var cs in all)
            {
                var column = table.ModifiedColumns[cs.ColumnName];
                column.Modifications.Current.Type = ExtractType(cs);
                column.Modifications.Current.Collation = cs.CollationName;
            }
        }


        private string ExtractType(ColumnSchema schema)
        {
            var rez = schema.DataType;
            if (!string.IsNullOrEmpty(schema.CharacterMaximumLength))
            {
                rez = rez + "(" + (schema.CharacterMaximumLength == "-1"
                                       ? "max"
                                       : schema.CharacterMaximumLength) + ")";
                return rez;
            }
            if (!string.IsNullOrEmpty(schema.NumericPrecision))
            {
                rez = rez + "(" + schema.NumericPrecision;
                if (!string.IsNullOrEmpty(schema.NumericScale))
                {
                    rez = rez + "," + schema.NumericScale;
                }
                return rez + ")";
            }
            return rez;
        }
    }
}