using System;

namespace SqlFu.Configuration.Internals
{
    public class TableExistsException : Exception
    {
        public TableExistsException(string name)
            : base(string.Format("A table named '{0}' already exists", name))
        {
        }
    }
}