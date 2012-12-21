namespace SqlFu.DDL
{
    public interface IModifyTable : ICompleteTableDefinition, ISupportSpecificTableOptions<IModifyTable>
    {
        IModifyColumns Columns { get; }
        IModifyConstraints Constraints { get; }
        IModifyIndexes Indexes { get; }
    }
}