namespace SqlFu.DDL
{
    public interface ICreateIndexes:ISupportSpecificIndexOptions
    {
        ISupportSpecificIndexOptions AddIndexOn(string columnsNames, bool isUnique, string indexName = null);
        ISupportSpecificIndexOptions this[string idxName] { get; }
    }

    
    public interface ISupportSpecificIndexOptions
    {
        IDefineSpecificIndexOptions IfDatabaseIs(DbEngine engine);
    }

    public interface IDefineSpecificIndexOptions:ISupportSpecificIndexOptions
    {
        /// <summary>
        /// Set db engine specific options
        /// </summary>
        /// <param name="options"> </param>
        /// <returns></returns>
        IDefineSpecificIndexOptions SetOptions(params DbSpecificOption[] options);

        IDefineSpecificIndexOptions RedefineAs(string definition);
        IDefineSpecificIndexOptions AddCustom(string definition);
        IDefineSpecificIndexOptions AddCustom(DbEngine engine, string definition);
    }
}