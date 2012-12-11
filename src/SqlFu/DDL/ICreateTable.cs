namespace SqlFu.DDL
{
    public interface ICreateTable:ICompleteTableDefinition,ISupportSpecificTableOptions<ICreateTable>
    {
        IAddColumns Columns { get; }

        ICreateConstraints Constraints { get; }

        ICreateIndexes Indexes { get; }
              
    }
}