namespace SqlFu
{
    public interface IDbProviderExpressionHelper : IEscapeDbIdentifier
    {
        string FormatBoolean(bool value);
        string Substring(string column, int pos, int length);
        string ToUpper(string column);
        string ToLower(string column);
        string Length(string column);
        string Year(string column);
        string Day(string column);
    }
}