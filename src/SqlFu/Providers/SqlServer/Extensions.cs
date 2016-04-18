namespace SqlFu.Providers.SqlServer
{
    public static class Extensions
    {
        public static bool IsSqlserver(this IDbProvider prov)
            => prov.ProviderId == SqlServer2012Provider.Id;
            
    }
}