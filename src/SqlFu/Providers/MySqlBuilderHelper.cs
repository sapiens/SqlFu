using System;

namespace SqlFu.Providers
{
    internal class MySqlBuilderHelper : CommonBuilderHelper
    {
        public override string EscapeName(string s)
        {
            return MySqlProvider.EscapeIdentifier(s);
        }


        public override string Substring(string column, int pos, int length)
        {
            column.MustNotBeNull();
            var idx = pos + 1;
            return "substr({0},{1},{2})".ToFormat(EscapeName(column), idx, length);
        }


        public override string Length(string column)
        {
            return "length({0})".ToFormat(EscapeName(column));
        }


        public override string Day(string column)
        {
            return "DAYOFMONTH({0})".ToFormat(EscapeName(column));
        }
    }
}