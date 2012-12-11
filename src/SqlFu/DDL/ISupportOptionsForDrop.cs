namespace SqlFu.DDL
{
    public interface ISupportOptionsForDrop<T>
    {
        T WithOptionsForDatabase(DbEngine engine, params DbSpecificOption[] options);
        IModifyTable WithoutOptions();
    }
  
}