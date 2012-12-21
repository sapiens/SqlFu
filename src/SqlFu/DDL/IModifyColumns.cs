namespace SqlFu.DDL
{
    public interface IModifyColumns : IAddColumns
    {
        IModifyColumns RenameColumn(string currentName, string newName);
        IChangeColumn Change(string name);
        ISupportOptionsForDrop<IModifyColumns> Drop(string name);
    }
}