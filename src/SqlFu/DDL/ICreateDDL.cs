namespace SqlFu.DDL
{
    public interface ICreateDDL
    {
        /// <summary>
        /// Creates a fluent builder object to help you generate a Create Table script
        /// </summary>
        /// <param name="name">Table name</param>
        /// <param name="option">What happens if table exists</param>
        /// <returns></returns>
        ICreateTable GetCreateTableBuilder(string name, IfTableExists option = IfTableExists.Throw);

        /// <summary>
        /// Creates a fluent builder object to help you generate a Create Table script, starting from an existing type.
        /// You can customize it further
        /// </summary>
        /// <returns></returns>
        ICreateTable GetCreateTableBuilder<T>();

        /// <summary>
        /// Creates a fluent builder object to help you generate an Alter Table script
        /// </summary>
        /// <param name="name">Table name</param>
        /// <returns></returns>
        IModifyTable GetAlterTableBuilder(string name);
    }
}