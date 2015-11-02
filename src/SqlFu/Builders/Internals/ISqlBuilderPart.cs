namespace SqlFu.Builders.Internals
{
    public interface ISqlBuilderPart
    {
        string ToString();
        string PartId { get; }
    }
}