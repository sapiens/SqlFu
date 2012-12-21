namespace SqlFu.DDL
{
    public interface IModifyIndexes : ICreateIndexes
    {
        ISupportOptionsForDrop<IModifyIndexes> Drop(string idxName);
    }
}