using System;

namespace SqlFu.Providers.SqlServer
{
    public static class Extensions
    {
        public static bool IsSqlserver(this IDbProvider prov)
            => prov.ProviderId == SqlServer2012Provider.Id;

        public static int DateDiff<T>(this T table, TSqlDatePart part, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

    }
}