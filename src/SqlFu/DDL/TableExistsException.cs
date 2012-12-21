using System;

namespace SqlFu.DDL
{
    public class TableExistsException : Exception
    {
        public TableExistsException(string name) : base(string.Format("A table named '{0}' already exists", name))
        {
        }
    }
}