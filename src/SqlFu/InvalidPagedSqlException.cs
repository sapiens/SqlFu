using System;

namespace SqlFu
{
    public class InvalidPagedSqlException : Exception
    {
        public InvalidPagedSqlException(string sql)
            : base(string.Format("String '{0}' is not suitable for paging", sql))
        {
        }
    }
}