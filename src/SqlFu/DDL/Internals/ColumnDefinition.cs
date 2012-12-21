using System.Collections.Generic;
using System.Data;

namespace SqlFu.DDL.Internals
{
    internal class ColumnDefinition
    {
        public ColumnDefinition()
        {
            Options = new DbEngineOptions();
        }

        public string Name { get; set; }

        public DbType Type { get; set; }

        /// <summary>
        /// Default is false
        /// </summary>
        public bool IsNullable { get; set; }

        public string DefaultValue { get; set; }

        public string Size { get; set; }
        public bool IsIdentity { get; set; }
        public string Collate { get; set; }
        internal DbEngineOptions Options { get; private set; }

        private readonly Dictionary<DbEngine, string> _redefined = new Dictionary<DbEngine, string>();


        public void Redefine(DbEngine engine, string definition)
        {
            _redefined[engine] = definition;
        }

        public bool IsRedefined(DbEngine engine)
        {
            return _redefined.ContainsKey(engine);
        }

        public string GetDefinition(DbEngine engine)
        {
            return _redefined[engine];
        }
    }
}