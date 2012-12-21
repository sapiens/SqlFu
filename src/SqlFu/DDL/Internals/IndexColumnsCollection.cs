using System.Collections.Generic;

namespace SqlFu.DDL.Internals
{
    internal class IndexColumnsCollection : List<IndexColumn>
    {
        public IndexColumn this[string name]
        {
            get { return Find(d => d.Name == name); }
        }

        public void AddColumns(string columns)
        {
            foreach (var name in columns.Split(','))
            {
                Add(new IndexColumn(name.Trim()));
            }
        }
    }
}