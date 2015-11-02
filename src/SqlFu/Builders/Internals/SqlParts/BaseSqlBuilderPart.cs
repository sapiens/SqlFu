namespace SqlFu.Builders.Internals.SqlParts
{
    public abstract class BaseSqlBuilderPart : ISqlBuilderPart
    {
        protected BaseSqlBuilderPart(string partId)
        {
            PartId = partId;
        }

        public string PartId { get; private set; }
    }
}