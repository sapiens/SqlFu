using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlColumnChangesWriter : AbstractColumnChangesWriter
    {
        public MySqlColumnChangesWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        public override void WriteDropDefault(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} alter {1} drop default;",
                                 MySqlProvider.EscapeIdentifier(col.TableName), MySqlProvider.EscapeIdentifier(col.Name));
        }

        /// <summary>
        /// type, size,collate, null
        /// </summary>
        public override void WriteColumnChanges(ColumnModifications col)
        {
            var w = new MySqlColumnWriter(Builder);
            Builder.AppendFormat("alter table {0} modify ", MySqlProvider.EscapeIdentifier(col.Name));
            w.Write(col);
        }

        public override void WriteSetDefault(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} alter {1} set default '{2}';",
                                 MySqlProvider.EscapeIdentifier(col.TableName), MySqlProvider.EscapeIdentifier(col.Name),
                                 col.DefaultValue);
        }
    }
}