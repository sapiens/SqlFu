namespace SqlFu.Providers.SqlServer
{
    internal class SqlServerBuilderHelper : CommonBuilderHelper
    {
        public override string EscapeName(string s)
        {
            return SqlServerProvider.EscapeIdentifier(s);
        }
    }
}