using System;

namespace SqlFu.Builders
{
    public interface IWriteColumnDefinition:IDisposable
    {
        IWriteColumnDefinition WriteNameAndType();
        IWriteColumnDefinition WriteSize();
        IWriteColumnDefinition WriteCollation();
        IWriteColumnDefinition WriteNull();
        IWriteColumnDefinition WriteDefault();      
    }
}