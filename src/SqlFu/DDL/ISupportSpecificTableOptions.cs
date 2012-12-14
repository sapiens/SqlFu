namespace SqlFu.DDL
{
    public interface ISupportSpecificTableOptions<T>
    {
        /// <summary>
        /// Set table options for the specified db engine
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        T TableOptionsFor(DbEngine engine, params DbSpecificOption[] options);
    }
}