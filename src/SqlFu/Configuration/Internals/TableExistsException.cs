using System;

namespace SqlFu.Configuration.Internals
{
    public class TableExistsException : Exception
    {
        public TableExistsException(TableName table)
            : base($"A table named '{table}' already exists")
        {
        }
    }
}