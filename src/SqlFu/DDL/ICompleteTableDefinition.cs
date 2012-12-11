namespace SqlFu.DDL
{
    public interface ICompleteTableDefinition
    {
        string GetSql();
        void ExecuteDDL();
    }
}