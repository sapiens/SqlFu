namespace SqlFu.DDL
{
    public interface ISupportSpecificTableOptions<T>
    {
        T TableOptionsFor(DbEngine engine, params DbSpecificOption[] options);
    }
}