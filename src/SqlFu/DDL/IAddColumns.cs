using System.Collections.Generic;
using System.Data;

namespace SqlFu.DDL
{
    public interface IAddColumns : ISupportSpecificColumnsOptions
    {
        /// <summary>
        /// Adds a new column to table
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="type">Column type</param>
        /// <param name="size">leavy empty for max size</param>
        /// <param name="isNullable"></param>
        /// <param name="defaultValue"></param>
        /// <param name="collation"> </param>
        /// <param name="autoIncrement">identity column</param>
        /// <returns></returns>
        IConfigureColumns Add(string name, DbType type, string size = "", bool isNullable = true,
                              string defaultValue = "", string collation = "", bool autoIncrement = false);
    }

    public interface IIndexForCreateColumn
    {
        /// <summary>
        /// Gets columns options if the column has been defined in the builder
        /// </summary>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <param name="name"></param>
        /// <returns></returns>
        ISupportSpecificColumnsOptions this[string name] { get; }
    }

    public interface ICreateColumns : IIndexForCreateColumn, IAddColumns
    {
    }

    public interface IConfigureColumns : IAddColumns
    {
        /// <summary>
        /// Set the primary key constraint
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        IConfigureColumns AsPrimaryKey(string keyName = null);

        /// <summary>
        /// Column has unique values
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IConfigureColumns AsUnique(string name = null);

        /// <summary>
        /// Column value must respect expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="constraintName"></param>
        /// <returns></returns>
        IConfigureColumns WithCheck(string expression, string constraintName);

        /// <summary>
        /// Create an index for column
        /// </summary>
        /// <param name="idxName"></param>
        /// <param name="unique"></param>
        /// <returns></returns>
        IConfigureColumns WithIndex(string idxName = null, bool unique = false);

        /// <summary>
        /// Column values are foreign keys
        /// </summary>
        /// <param name="parentTable">Table containing values for the column</param>
        /// <param name="parentColumns">Columns of the parent table</param>
        /// <param name="onUpdate"></param>
        /// <param name="onDelete"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        IConfigureColumns IsForeignKeyFrom(string parentTable, string parentColumns,
                                           ForeignKeyRelationCascade onUpdate = ForeignKeyRelationCascade.NoAction,
                                           ForeignKeyRelationCascade onDelete = ForeignKeyRelationCascade.NoAction,
                                           string keyName = null);
    }


    public interface ISupportSpecificColumnsOptions
    {
        /// <summary>
        /// Customize column for a specific database engine
        /// </summary>
        /// <param name="engine"></param>
        /// <returns></returns>
        IDefineSpecificColumnsOptions IfDatabaseIs(DbEngine engine);
    }


    public interface IDefineSpecificColumnsOptions : IAddColumns
    {
        /// <summary>
        /// Column will be defined acording to the specfied definition.
        ///  </summary>
        /// <param name="definition">Definition of the column excluding column name</param>
        /// <returns></returns>
        IDefineSpecificColumnsOptions RedefineColumnAs(string definition);

        /// <summary>
        /// Set db engine specific options
        /// </summary>
        /// <param name="options"> </param>
        /// <returns></returns>
        IDefineSpecificColumnsOptions SetColumnOptions(params DbSpecificOption[] options);
    }
}