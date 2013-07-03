using System;

namespace SqlFu.Providers.SqlServer
{
    internal class SqlServerCEBuilderHelper : SqlServerBuilderHelper
    {
        public override string Year(string column)
        {
            return "datepart(year,{0})".ToFormat(EscapeName(column));
        }

        public override string Day(string column)
        {
            return "datepart(day,{0})".ToFormat(EscapeName(column));
        }
    }
}