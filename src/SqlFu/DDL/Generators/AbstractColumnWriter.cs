using System;
using System.Data;
using System.Text;
using SqlFu.DDL.Internals;

namespace SqlFu.DDL.Generators
{
    internal abstract class AbstractColumnWriter : AbstractSchemaItemWriter
    {
        public AbstractColumnWriter(StringBuilder builder, DbEngine engine) : base(builder, engine)
        {
        }

        protected ColumnDefinition Definition
        {
            get { return _definition; }
        }

        public virtual void Write(ColumnModifications col)
        {
            if (col.Type != null)
            {
                Builder.Append(GetType(col.Type.Value, col.Size));
            }
            else
            {
                Builder.Append(col.Current.Type);
            }

            WriteCollation(col.Collation);

            if (col.Nullable != null)
            {
                WriteNullable(col.Nullable.Value);
            }

            WriteEndColumnOptions(col.Options);
        }

        protected virtual void WriteEndColumnOptions(DbEngineOptions options)
        {
        }

        private ColumnDefinition _definition;

        public virtual void Write(ColumnDefinition col)
        {
            if (RedefineHandled(col))
            {
                return;
            }
            _definition = col;
            col.Options.Use(Engine);

            WriteNameAndType(col);

            WriteNullable(col.IsNullable);

            if (!col.IsIdentity) WriteDefault(col.DefaultValue);

            else WriteIdentity(col);
        }

        protected abstract void WriteIdentity(ColumnDefinition col);

        protected virtual void WriteDefault(string value)
        {
            if (!value.IsNullOrEmpty())
            {
                Int64 i;
                if (!Int64.TryParse(value, out i))
                {
                    value = "'" + value + "'";
                }
                Builder.AppendFormat(" DEFAULT {0}", value);
            }
        }

        protected virtual void WriteNullable(bool nullable)
        {
            if (!nullable)
            {
                Builder.Append(" NOT");
            }

            Builder.Append(" NULL");
        }

        protected bool RedefineHandled(ColumnDefinition col)
        {
            if (col.IsRedefined(Engine))
            {
                Builder.Append(EscapeName(col.Name) + " " + col.GetDefinition(Engine));
                return true;
            }
            return false;
        }

        protected virtual void WriteNameAndType(ColumnDefinition col)
        {
            Builder.AppendFormat("{0} {1}", EscapeName(col.Name), GetType(col.Type, col.Size));
            WriteCollation(col.Collate);
        }

        protected void WriteCollation(string collate)
        {
            if (!collate.IsNullOrEmpty())
            {
                Builder.AppendFormat(" COLLATE {0}", collate);
            }
        }

        protected abstract string EscapeName(string name);

        #region DbType

        protected string GetType(DbType type, string size)
        {
            switch (type)
            {
                case DbType.DateTimeOffset:
                    return DateTimeOffset(size);
                case DbType.Boolean:
                    return Boolean();
                case DbType.AnsiString:
                    return AnsiString(size);
                case DbType.AnsiStringFixedLength:
                    return AnsiStringFixedLength(size);
                case DbType.Binary:
                    return Binary(size);
                case DbType.Byte:
                    return Byte();
                case DbType.Currency:
                    return Currency();
                case DbType.Date:
                    return Date();
                case DbType.Int32:
                    return Int32();
                case DbType.String:
                    return String(size);
                case DbType.StringFixedLength:
                    return StringFixedLength(size);
                case DbType.Xml:
                    return Xml();
                case DbType.DateTime:
                case DbType.DateTime2:
                    return DateTime();
                case DbType.Guid:
                    return Guid();
                case DbType.Decimal:
                    return Decimal(size);
                case DbType.Double:
                    return Double();
                case DbType.Int16:
                    return SmallInt();
                case DbType.Int64:
                    return BigInt();
                case DbType.SByte:
                    return SByte();
                case DbType.Single:
                    return Single(size);
                case DbType.Time:
                    return Time(size);
                case DbType.VarNumeric:
                    return VarNumeric(size);
                case DbType.UInt16:
                    return UInt16();
                case DbType.UInt32:
                    return UInt32();
                case DbType.UInt64:
                    return UInt64();
            }
            throw new NotSupportedException();
        }

        protected abstract string UInt16();
        protected abstract string UInt32();
        protected abstract string UInt64();

        protected virtual string VarNumeric(string size)
        {
            var rez = "numeric";
            if (!size.IsNullOrEmpty())
            {
                rez += string.Format("({0})", size);
            }
            return rez;
        }

        protected virtual string SByte()
        {
            return "tinyint";
        }

        protected virtual string Single(string size)
        {
            return "real";
        }

        protected virtual string SmallInt()
        {
            return "smallint";
        }

        protected virtual string BigInt()
        {
            return "bigint";
        }

        protected abstract string Guid();

        protected virtual string Double()
        {
            return "double";
        }

        protected virtual string Decimal(string size)
        {
            var rez = "decimal";
            if (!size.IsNullOrEmpty())
            {
                rez += string.Format("({0})", size);
            }
            return rez;
        }

        protected virtual string Time(string size)
        {
            var rez = "time";
            if (!size.IsNullOrEmpty())
            {
                rez += string.Format("({0})", size);
            }
            return rez;
        }

        protected virtual string DateTime()
        {
            return "datetime";
        }

        protected virtual string Xml()
        {
            throw new NotSupportedException("Redefine the column for the target database");
        }

        protected abstract string StringFixedLength(string size);


        protected abstract string String(string size);


        protected virtual string Date()
        {
            return "date";
        }

        protected virtual string Int32()
        {
            return "int";
        }

        protected abstract string Binary(string size);

        protected abstract string Currency();

        protected abstract string AnsiStringFixedLength(string size);

        protected abstract string AnsiString(string size);

        protected abstract string Boolean();

        protected virtual string Byte()
        {
            return "tinyint";
        }

        protected abstract string DateTimeOffset(string size);

        #endregion;       
    }
}