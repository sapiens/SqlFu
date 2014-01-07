using System.Text;
using SqlFu.DDL.Internals;
using SqlFu.Providers;

namespace SqlFu.DDL.Generators.Sqlite
{
    internal class SqliteColumnWriter : AbstractColumnWriter
    {
        public SqliteColumnWriter(StringBuilder builder) : base(builder, DbEngine.SQLite)
        {
        }

        protected override string EscapeName(string name)
        {
            return SqliteProvider.EscapeIdentifier(name);
        }


        protected override void WriteEndColumnOptions(DbEngineOptions options)
        {
            WriteOnConflictOption(options);
        }

        private void WriteOnConflictOption(DbEngineOptions options)
        {
            var opt = options.Get(SqliteOptions.On_Conflict);
            if (opt != null)
            {
                Builder.Append(" " + opt);
            }
        }

        protected override void WriteNullable(bool nullable)
        {
            base.WriteNullable(nullable);
            if (Definition != null)
            {
                if (!Definition.IsIdentity)
                {
                    WriteOnConflictOption(Definition.Options);
                }
            }
        }

        protected override void WriteIdentity(ColumnDefinition col)
        {
            Builder.Append(" primary key autoincrement");
        }

        #region DbTypes

        protected override string UInt16()
        {
            return "integer";
        }

        protected override string UInt32()
        {
            return "integer";
        }

        protected override string UInt64()
        {
            return "integer";
        }

        protected override string Guid()
        {
            return AnsiStringFixedLength("36");
        }

        protected override string StringFixedLength(string size)
        {
            return String(size);
        }

        protected override string String(string size)
        {
            var rez = "text";
            if (!string.IsNullOrEmpty(size)) rez = rez + "(" + size + ")";
            return rez;
        }

        protected override string Binary(string size)
        {
            var rez = "blob";
            if (!string.IsNullOrEmpty(size)) rez = rez + "(" + size + ")";
            return rez;
        }

        protected override string Currency()
        {
            return "numeric";
        }

        protected override string AnsiStringFixedLength(string size)
        {
            return String(size);
        }

        protected override string AnsiString(string size)
        {
            return String(size);
        }

        protected override string Boolean()
        {
            return Int32();
        }

        protected override string Int32()
        {
            return "integer";
        }
        
		protected override string SmallInt()
		{
			return "integer";
		}
		
		protected override string BigInt()
		{
			return "integer";
		}
		

        protected override string DateTimeOffset(string size)
        {
            return String(null);
        }
        #endregion
    }
}