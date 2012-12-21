using System;
using System.Collections.Generic;

namespace SqlFu.DDL.Internals
{
    internal class IndexDefinition
    {
        public IndexDefinition(string table)
        {
            Options = new DbEngineOptions();
            TableName = table;
            Columns = new IndexColumnsCollection();
        }

        internal DbEngineOptions Options { get; private set; }
        public string Name { get; set; }
        public string TableName { get; set; }
        public bool IsUnique { get; set; }


        public IndexColumnsCollection Columns { get; private set; }
        private readonly Dictionary<DbEngine, string> _redefined = new Dictionary<DbEngine, string>();

        public bool IsRedefinedFor(DbEngine engine)
        {
            return _redefined.ContainsKey(engine);
        }

        public string GetDefinition(DbEngine engine)
        {
            return _redefined[engine];
        }

        public void Redefine(DbEngine engine, string definition)
        {
            definition.MustNotBeEmpty();
            _redefined[engine] = definition;
        }
    }
}