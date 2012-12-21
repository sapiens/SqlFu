using System;

namespace SqlFu.DDL
{
    public class IncompatibleOptionException : Exception
    {
        public IncompatibleOptionException(DbEngine current, DbEngine restricted)
            : base(string.Format("You have specific options for '{0}' but you are using '{1}'.", restricted, current))
        {
        }
    }
}