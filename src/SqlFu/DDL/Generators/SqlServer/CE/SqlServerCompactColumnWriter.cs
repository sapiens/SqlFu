using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactColumnWriter : SqlServerColumnWriter
    {
        public SqlServerCompactColumnWriter(StringBuilder builder) : base(builder, DbEngine.SqlServerCE)
        {
        }

        public override void Write(ColumnModifications col)
        {
            col.Collation = null;
            base.Write(col);
        }

        protected override string DateTimeOffset(string size)
        {
            return base.AnsiString("40");
        }

        protected override string Date()
        {
            return base.DateTime();
        }

        protected override void WriteSparseOption(DbEngineOptions options)
        {
        }
    }
}