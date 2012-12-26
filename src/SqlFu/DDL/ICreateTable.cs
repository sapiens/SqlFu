namespace SqlFu.DDL
{
    public interface ICreateTable : ICompleteTableDefinition, ISupportSpecificTableOptions<ICreateTable>
    {
        /// <summary>
        /// Define table columns
        /// </summary>
        ICreateColumns Columns { get; }

        /// <summary>
        /// Define table constraints
        /// </summary>
        ICreateConstraints Constraints { get; }

        /// <summary>
        /// Define table indexes
        /// </summary>
        ICreateIndexes Indexes { get; }
    }
}