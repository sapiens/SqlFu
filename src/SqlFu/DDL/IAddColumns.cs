using System.Data;

namespace SqlFu.DDL
{

    public interface IAddColumns : ISupportSpecificColumnsOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="size"></param>
        /// <param name="isNullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="collation"> </param>
        /// <param name="autoIncrement"> </param>
        /// <returns></returns>
        IConfigureColumns Add(string name, DbType type, string size = "", bool isNullable = true, string defaultValue = "", string collation = "", bool autoIncrement = false);

       // IConfigureColumns this[string name] { get; }

    }

    public interface IConfigureColumns:IAddColumns
    {
        IConfigureColumns AsPrimaryKey(string keyName=null);
        IConfigureColumns AsUnique(string name=null);
        IConfigureColumns WithCheck(string expression, string constraintName);
        IConfigureColumns WithIndex(string idxName = null, bool unique = false);

        IConfigureColumns IsForeignKeyFrom(string parentTable, string parentColumns,
                                          ForeignKeyRelationCascade onUpdate = ForeignKeyRelationCascade.NoAction,
                                          ForeignKeyRelationCascade onDelete = ForeignKeyRelationCascade.NoAction,
                                          string keyName = null);
    }

    
    public interface ISupportSpecificColumnsOptions
    {
        IDefineSpecificColumnsOptions IfDatabaseIs(DbEngine engine);
    }


    public interface IDefineSpecificColumnsOptions : IAddColumns
    {
        IDefineSpecificColumnsOptions RedefineColumnAs(string definition);

        /// <summary>
        /// Set db engine specific options
        /// </summary>
        /// <param name="options"> </param>
        /// <returns></returns>
        IDefineSpecificColumnsOptions SetColumnOptions(params DbSpecificOption[] options);
    }
}