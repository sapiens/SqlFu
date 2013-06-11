using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlFu.DDL.Internals
{
    internal class IndexCollection : List<IndexDefinition>
    {
        private readonly TableSchema _table;

        public IndexCollection(TableSchema table)
        {
            _table = table;
            Dropped = new DroppedSchemaItemsCollection(table.Name);
        }

        public IndexDefinition AddIndex(string columnsNames, bool isUnique = false, string indexName = null)
        {
            columnsNames.MustNotBeEmpty();
            var idx = new IndexDefinition(_table.Name)
                {
                    IsUnique = isUnique,
                    Name = indexName ?? GenerateIndexName(columnsNames)
                };
            idx.Columns.AddColumns(columnsNames);
            Add(idx);
            return idx;
        }

        public IndexDefinition this[string indexName]
        {
            get { return Find(d => d.Name == indexName); }
        }

        private readonly List<Tuple<DbEngine, string>> _added = new List<Tuple<DbEngine, string>>();

        public void AddSpecific(DbEngine engine, string definition)
        {
            definition.MustNotBeEmpty();
            _added.Add(new Tuple<DbEngine, string>(engine, definition));
        }

        public IEnumerable<string> GetSpecificIndexes(DbEngine engine)
        {
            return _added.FindAll(d => d.Item1 == engine).Select(d => d.Item2);
        }

        private string GenerateIndexName(string columns)
        {
            return "IX_" + _table.TableName.FilterEscape() + "_" + _table.ColumnsToName(columns).FilterEscape();
        }

        public DroppedSchemaItemsCollection Dropped { get; private set; }
    }
}