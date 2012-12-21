using System.Data;

namespace SqlFu.DDL
{
    public interface IChangeColumn
    {
        IChangeColumn SetType(DbType type, string size = null);
        IChangeColumn RenameTo(string name);
        IChangeColumn SetCollation(string collation);
        IChangeColumn SetDefault(string defaultValue);
        IChangeColumn SetNullable(bool isNullable);
        IChangeColumn DropDefault();
        // IChangeColumn SetOptionsFor(DbEngine engine, params string[] options);
        //  IChangeColumn RemoveOptionsFor(DbEngine engine, params string[] options);
    }
}