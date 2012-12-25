using System;
using System.Data;
using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Postgresql
{
    internal class PostgresColumnWriter : AbstractColumnWriter
    {
        public PostgresColumnWriter(StringBuilder builder) : base(builder, DbEngine.PostgreSQL)
        {
        }

        public override void Write(ColumnDefinition col)
        {
            if (col.IsIdentity)
            {
                Builder.AppendFormat("{0} serial not null", EscapeName(col.Name));
            }
            else base.Write(col);
        }

        protected override void WriteDefault(string value)
        {
            if (Definition.Type == DbType.Boolean)
            {
                if (value == "0") value = "false";
                else if (value == "1") value = "true";
            }
            base.WriteDefault(value);
        }

        protected override void WriteIdentity(ColumnDefinition col)
        {
        }

        protected override string SByte()
        {
            return Byte();
        }

        protected override string Double()
        {
            return "double precision";
        }

        protected override string DateTime()
        {
            return "timestamp";
        }

        protected override string EscapeName(string name)
        {
            return PostgresProvider.EscapeIdentifier(name);
        }

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

        protected override string Byte()
        {
            return "smallint";
        }

        protected override string Guid()
        {
            return "uuid";
        }

        protected override string StringFixedLength(string size)
        {
            if (size.IsNullOrEmpty()) return "text";
            return "char(" + size + ")";
        }

        protected override string String(string size)
        {
            if (size.IsNullOrEmpty()) return "text";
            return "varchar(" + size + ")";
        }

        protected override string Binary(string size)
        {
            if (size.IsNullOrEmpty() || size.Equals("max")) return "bytea";
            return "bytea(" + size + ")";
        }

        protected override string Currency()
        {
            return "money";
        }

        protected override string AnsiStringFixedLength(string size)
        {
            return StringFixedLength(size);
        }

        protected override string AnsiString(string size)
        {
            return String(size);
        }

        protected override string Boolean()
        {
            return "boolean";
        }

        protected override string DateTimeOffset(string size)
        {
            if (size.IsNullOrEmpty()) size = "6";
            return "timestamp (" + size + ") with time zone";
        }
    }
}