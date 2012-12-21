using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresColumnChangesWriter : AbstractColumnChangesWriter
    {
        public PostgresColumnChangesWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        public override void WriteDropDefault(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} alter {1} drop default;\n\r", col.TableName, col.Name);
        }

        /// <summary>
        /// type, size,collate, null
        /// </summary>
        public override void WriteColumnChanges(ColumnModifications col)
        {
            var w = new PostgresColumnWriter(Builder);
            w.Write(col);
        }

        public override void WriteSetDefault(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} alter {1} set default {2};\n\r", col.TableName, col.Name,
                                 col.DefaultValue);
        }
    }
}