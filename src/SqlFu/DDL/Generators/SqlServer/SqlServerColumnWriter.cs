using System;
using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer
{
    internal class SqlServerColumnWriter : AbstractColumnWriter
    {
        public SqlServerColumnWriter(StringBuilder builder) : base(builder, DbEngine.SqlServer)
        {
        }

        public SqlServerColumnWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected override void WriteIdentity(ColumnDefinition col)
        {
            Builder.Append(" IDENTITY(1,1)");
        }

        protected override string EscapeName(string name)
        {
            return SqlServerProvider.EscapeIdentifier(name);
        }

        #region DB Types

        protected override string UInt16()
        {
            return SmallInt();
        }

        protected override string UInt32()
        {
            return Int32();
        }

        protected override string UInt64()
        {
            return BigInt();
        }

        protected override string Guid()
        {
            return "uniqueidentifier";
        }


        protected override string StringFixedLength(string size)
        {
            if (size.IsNullOrEmpty()) size = "max";
            return "nchar(" + size + ")";
        }

        protected override string String(string size)
        {
            if (size.IsNullOrEmpty()) size = "max";
            return "nvarchar(" + size + ")";
        }

        protected override string Binary(string size)
        {
            if (size.IsNullOrEmpty()) size = "max";
            return "varbinary(" + size + ")";
        }

        protected override string Currency()
        {
            return "money";
        }

        protected override string AnsiStringFixedLength(string size)
        {
            if (size.IsNullOrEmpty()) size = "1";
            return "char(" + size + ")";
        }

        protected override string AnsiString(string size)
        {
            if (size.IsNullOrEmpty()) size = "1";
            return "varchar(" + size + ")";
        }


        protected override string Boolean()
        {
            return "bit";
        }

        protected override string Double()
        {
            return "float";
        }

        protected override string DateTimeOffset(string size)
        {
            var hasSize = !size.IsNullOrEmpty();
            var rez = "datetimeoffset";
            if (hasSize)
            {
                rez += string.Format("({0})", size);
            }
            return rez;
        }

        #endregion

        protected override void WriteNullable(bool nullable)
        {
            if (Definition != null) WriteSparseOption(Definition.Options);
            base.WriteNullable(nullable);
        }

        protected virtual void WriteSparseOption(DbEngineOptions options)
        {
            var option = options.Get(SqlServerOptions.Sparse);
            if (option != null)
            {
                Builder.Append(" " + option);
            }
        }

        protected override void WriteEndColumnOptions(DbEngineOptions options)
        {
            var option = options.Get(SqlServerOptions.RowGuidCol);
            if (option != null)
            {
                Builder.Append(" " + option);
            }
        }
    }
}