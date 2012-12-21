using System;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteColumnChangesWriter : AbstractColumnChangesWriter
    {
        public SqliteColumnChangesWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        public override void WriteDropDefault(ColumnModifications col)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// type, size,collate, null
        /// </summary>
        public override void WriteColumnChanges(ColumnModifications col)
        {
            throw new NotImplementedException();
        }

        public override void WriteSetDefault(ColumnModifications col)
        {
            throw new NotImplementedException();
        }
    }
}