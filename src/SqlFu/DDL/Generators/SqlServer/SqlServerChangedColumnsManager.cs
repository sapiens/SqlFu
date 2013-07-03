using System.Data.Common;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerChangedColumnsManager : AbstractChangedColumnsManager
    {
        private readonly DbConnection _db;

        public SqlServerChangedColumnsManager(StringBuilder builder, DbConnection db)
            : base(builder, DbEngine.SqlServer)
        {
            _db = db;
        }

        #region Overrides of AbstractColumnsChangeManager

        public override void Write(ModifiedColumnsCollection columns)
        {
            if (!columns.HasAnyChange) return;
            FillRealTableSchema(columns);
            base.Write(columns);
        }

        protected override bool ShouldDropDefault(ColumnModifications col)
        {
            return base.ShouldDropDefault(col) || col.HasChangedStructure || col.IsDropped;
        }

        protected override AbstractColumnChangesWriter GetWriter()
        {
            return new SqlServerColumnChangesWriter(Builder);
        }

        #endregion

        private void FillRealTableSchema(ModifiedColumnsCollection cols)
        {
            var columns = cols.AllColumnsNames;
            var all = _db.Query<ColumnSchema>(
                @"select COLUMN_NAME as ColumnName, DATA_TYPE as DataType
, cast (CHARACTER_MAXIMUM_LENGTH as varchar(5))as CharacterMaximumLength, cast (NUMERIC_PRECISION as varchar(5)) as NumericPrecision, cast (NUMERIC_SCALE as varchar(5)) as NumericScale, COLLATION_NAME as CollationName from INFORMATION_SCHEMA.COLUMNS
where TABLE_NAME = @0 and COLUMN_NAME in (@1)", cols.TableName, columns);
            foreach (var cs in all)
            {
                var column = cols[cs.ColumnName];
                column.Modifications.Current.Type = ExtractType(cs);
                column.Modifications.Current.Collation = cs.CollationName;
            }

            var defaults = _db.Query<dynamic>(@"select c.name as colname, d.name as defname
from sys.tables t
    join
    sys.default_constraints d
        on d.parent_object_id = t.object_id
    join
    sys.columns c
        on c.object_id = t.object_id
        and c.column_id = d.parent_column_id
where t.name = @0
and c.name in (@1)", cols.TableName, columns);
            foreach (var def in defaults)
            {
                var column = cols[def.colname];
                column.Modifications.Current.DefaultConstraintName = def.defname;
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