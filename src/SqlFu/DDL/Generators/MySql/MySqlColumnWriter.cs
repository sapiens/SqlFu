using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.MySql
{
    internal class MySqlColumnWriter : AbstractColumnWriter
    {
        public MySqlColumnWriter(StringBuilder builder) : base(builder, DbEngine.MySql)
        {
        }

        protected override void WriteIdentity(ColumnDefinition col)
        {
            Builder.Append(" auto_increment");
        }

        protected override string EscapeName(string name)
        {
            return MySqlProvider.EscapeIdentifier(name);
        }

        protected override string UInt16()
        {
            return "smallint unsigned";
        }

        protected override string UInt32()
        {
            return "int unsigned";
        }

        protected override string UInt64()
        {
            return "bigint unsigned";
        }

        protected override string Guid()
        {
            return AnsiStringFixedLength("36");
        }

        protected override string StringFixedLength(string size)
        {
            int s = 255;
            if (int.TryParse(size, out s))
            {
                if (s > 255) s = 255;
            }
            return "char(" + s + ")";
        }

        protected override string String(string size)
        {
            int s = 0;
            if (int.TryParse(size, out s))
            {
                if (s <= 255)
                {
                    return "varchar (" + s + ")";
                }

                if (s <= 65536)
                {
                    return "text";
                }

                if (s <= 16777216)
                {
                    return "mediumtext";
                }
                return "longtext";
            }
            return "text";
        }

        protected override string Binary(string size)
        {
            int s = 0;
            if (int.TryParse(size, out s))
            {
                if (s <= 255)
                {
                    return "binary (" + s + ")";
                }

                if (s <= 65536)
                {
                    return "blob";
                }

                if (s <= 16777216)
                {
                    return "mediumblob";
                }
                return "longblob";
            }
            return "blob";
        }

        protected override string Currency()
        {
            return "decimal";
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
            return "bit";
        }

        protected override string DateTimeOffset(string size)
        {
            return DateTime();
        }
    }
}