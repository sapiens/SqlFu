using System;

namespace SqlFu.Providers
{
    public abstract class CommonBuilderHelper : IDbProviderExpressionHelper
    {
        public abstract string EscapeName(string s);

        public virtual string FormatBoolean(bool value)
        {
            return value ? "1" : "0";
        }

        public virtual string Substring(string column, int pos, int length)
        {
            column.MustNotBeNull();
            var idx = pos + 1;
            return "substring({0},{1},{2})".ToFormat(EscapeName(column), idx, length);
        }

        public string ToUpper(string column)
        {
            return "upper({0})".ToFormat(EscapeName(column));
        }

        public string ToLower(string column)
        {
            return "lower({0})".ToFormat(EscapeName(column));
        }

        public virtual string Length(string column)
        {
            return "len({0})".ToFormat(EscapeName(column));
        }

        public virtual string Year(string column)
        {
            return "year({0})".ToFormat(EscapeName(column));
        }

        public virtual string Day(string column)
        {
            return "day({0})".ToFormat(EscapeName(column));
        }
    }
}