namespace SqlFu.DDL
{
    public interface ISupportOptionsForDrop<T>
    {
        T WithOptionsForDatabase(DbEngine engine, params DbSpecificOption[] options);

        /// <summary>
        /// No options for a db engine
        /// </summary>
        /// <returns></returns>
        IModifyTable WithoutOptions();
    }
}