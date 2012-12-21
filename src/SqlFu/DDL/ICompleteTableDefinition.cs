namespace SqlFu.DDL
{
    public interface ICompleteTableDefinition
    {
        /// <summary>
        /// Generate DDL script
        /// </summary>
        /// <returns></returns>
        string GetSql();

        /// <summary>
        /// Executes generated DDL script
        /// </summary>
        void ExecuteDDL();
    }
}