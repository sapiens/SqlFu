using System.Data.Common;

namespace SqlFu
{
    public static class ExceptionHelpers
    {
        /// <summary>
        /// Checks is the exception was thrown because an unique constraint was violated.
        /// Can be used to implement idempotency e.g you can treat primary key violations as duplicate operations
        /// </summary>
        /// <param name="db"></param>
        /// <param name="ex"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static bool IsUniqueViolation(this DbConnection db, DbException ex, string keyName = null)
            => db.Provider().IsUniqueViolation(ex, keyName);
      
    }
}