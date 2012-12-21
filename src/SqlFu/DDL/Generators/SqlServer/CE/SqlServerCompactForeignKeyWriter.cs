using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators.SqlServer.CE
{
    internal class SqlServerCompactForeignKeyWriter : SqlServerForeignKeyWriter
    {
        public SqlServerCompactForeignKeyWriter(StringBuilder builder)
            : base(builder, DbEngine.SqlServerCE)
        {
        }

        public override void Write(ForeignKeyConstraint key)
        {
            if (key.OnDelete != ForeignKeyRelationCascade.Cascade && key.OnDelete != ForeignKeyRelationCascade.NoAction)
                return;
            if (key.OnUpdate != ForeignKeyRelationCascade.Cascade && key.OnUpdate != ForeignKeyRelationCascade.NoAction)
                return;
            base.Write(key);
        }
    }
}