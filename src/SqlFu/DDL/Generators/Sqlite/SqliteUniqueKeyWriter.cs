using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteUniqueKeyWriter:AbstractUniqueKeyWriter
    {
        public SqliteUniqueKeyWriter(StringBuilder builder) : base(builder,DbEngine.SQLite)
        {
        }

        //public override void Write(UniqueKeyConstraint constraint)
        //{
        //    if (!constraint.Ignore)
        //    {
        //        base.Write(constraint);
        //    }
        //}

        protected override void WriteColumnsNames(string columns, StringBuilder builder)
        {
         SqliteDDLWriter.WriteColumnsNames(columns,builder,SqliteProvider.EscapeIdentifier);   
        }
    }
}