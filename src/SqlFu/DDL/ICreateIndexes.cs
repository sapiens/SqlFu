namespace SqlFu.DDL
{
    public interface ICreateIndexes : ISupportSpecificIndexOptions
    {
        /// <summary>
        /// Specifies that an index will be created for the specified columns
        /// </summary>
        /// <param name="columnsNames"></param>
        /// <param name="isUnique"></param>
        /// <param name="indexName"></param>
        /// <returns></returns>
        ISupportSpecificIndexOptions AddIndexOn(string columnsNames, bool isUnique, string indexName = null);

        /// <summary>
        /// Gets index options by index name. Index MUST have been already defined in the builder
        /// </summary>
        /// <param name="idxName"></param>
        /// <returns></returns>
        ISupportSpecificIndexOptions this[string idxName] { get; }
    }


    public interface ISupportSpecificIndexOptions
    {
        /// <summary>
        /// Customize index options for the specified db engine
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        IDefineSpecificIndexOptions IfDatabaseIs(DbEngine engine);
    }

    public interface IDefineSpecificIndexOptions : ISupportSpecificIndexOptions
    {
        /// <summary>
        /// Set db engine specific options
        /// </summary>
        /// <param name="options"> </param>
        /// <returns></returns>
        IDefineSpecificIndexOptions SetOptions(params DbSpecificOption[] options);

        /// <summary>
        /// Use this definition to define the index
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        IDefineSpecificIndexOptions RedefineAs(string definition);

        /// <summary>
        /// Adds an index only for the specified db engine
        /// </summary>
        /// <param name="definition">Index definition ddl</param>
        /// <returns></returns>
        IDefineSpecificIndexOptions AddCustom(string definition);

        /// <summary>
        /// Adds an index only for the specified db engine
        /// </summary>
        /// <param name="engine">Db engine</param>
        /// <param name="definition">Index definition ddl</param>
        /// <returns></returns>
        IDefineSpecificIndexOptions AddCustom(DbEngine engine, string definition);
    }
}