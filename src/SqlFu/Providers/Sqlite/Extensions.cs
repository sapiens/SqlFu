namespace SqlFu.Providers.Sqlite
{
    public static class Extensions
    {
        public static bool IsSqlite(this IDbProvider cnx)
            => cnx.ProviderId == SqliteProvider.Id;
    }
}