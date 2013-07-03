using System;

namespace SqlFu.Providers
{
    internal class PostgresBuilderHelper : CommonBuilderHelper
    {
        public override string EscapeName(string s)
        {
            return PostgresProvider.EscapeIdentifier(s);
        }

        public override string FormatBoolean(bool value)
        {
            return value ? "'t'" : "'f'";
        }

        public override string Substring(string column, int pos, int length)
        {
            return "substring({0} from {1} for {2})".ToFormat(EscapeName(column), pos + 1, length);
        }

        public override string Length(string column)
        {
            return "char_length({0})".ToFormat(EscapeName(column));
        }

        public override string Year(string column)
        {
            return "extract(YEAR from {0})".ToFormat(EscapeName(column));
        }

        public override string Day(string column)
        {
            return "extract(DAY from {0})".ToFormat(EscapeName(column));
        }
    }
}