namespace SqlFu.DDL
{
    public interface IModifyConstraints : ICreateConstraints
    {
        ISupportOptionsForDrop<IModifyConstraints> Drop(string name);
        ISupportOptionsForDrop<IModifyConstraints> DropPrimaryKey();
    }
}