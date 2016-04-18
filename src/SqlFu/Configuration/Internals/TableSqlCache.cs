using System;
using System.Collections.Generic;

namespace SqlFu.Configuration.Internals
{
    public class TableSqlCache
    {
        /// <summary>
        /// Cache for insert text
        /// </summary>
        public string InsertSql { get; set; }  

        private Lazy<Dictionary<int, PagedSqlCache>> _paged = new Lazy<Dictionary<int, PagedSqlCache>>();
        public PagedSqlCache GetPaged(string sql) => _paged.Value.GetValueOrDefault(sql.GetHashCode());

        public void AddPaged(string sql, PagedSqlCache cache)
        {
            _paged.Value[sql.GetHashCode()] = cache;
        }
    }
}