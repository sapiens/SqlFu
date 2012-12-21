using System;
using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerColumnChangesWriter : AbstractColumnChangesWriter
    {
        private readonly SqlServerColumnWriter _writer;

        public SqlServerColumnChangesWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
            _writer = new SqlServerColumnWriter(Builder);
        }

        #region Overrides of AbstractColumnChangesWriter

        public override void WriteDropDefault(ColumnModifications col)
        {
            if (!col.Current.DefaultConstraintName.IsNullOrEmpty())
            {
                Builder.AppendFormat("alter table {0} drop constraint {1};\n",
                                     SqlServerProvider.EscapeIdentifier(col.TableName),
                                     col.Current.DefaultConstraintName);
            }
        }

        public override void WriteColumnChanges(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} alter column {1} ", SqlServerProvider.EscapeIdentifier(col.TableName),
                                 SqlServerProvider.EscapeIdentifier(col.Current.Name));
            _writer.Write(col);
            Builder.AppendLine(";");
        }

        public override void WriteSetDefault(ColumnModifications col)
        {
            Builder.AppendFormat("alter table {0} add constraint {1} default ('{2}') for {3};\n",
                                 SqlServerProvider.EscapeIdentifier(col.TableName),
                                 string.Format("DF_{0}_{1}", col.TableName, col.Name), col.DefaultValue, col.Name);
        }

        #endregion
    }
}