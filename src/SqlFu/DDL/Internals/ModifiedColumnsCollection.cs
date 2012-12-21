using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SqlFu.DDL.Internals
{
    internal class ModifiedColumnsCollection : IEnumerable<ColumnModifications>
    {
        private readonly string _table;
        private readonly Dictionary<string, ColumnEditor> _columns = new Dictionary<string, ColumnEditor>();

        public ModifiedColumnsCollection(string table)
        {
            _table = table;
        }

        public ColumnEditor this[string name]
        {
            get
            {
                ColumnEditor rez = null;
                if (!_columns.TryGetValue(name.ToLowerInvariant(), out rez))
                {
                    rez = new ColumnEditor(name, TableName);
                    _columns.Add(name.ToLowerInvariant(), rez);
                }
                return rez;
            }
        }

        public ICollection<string> AllColumnsNames
        {
            get { return _columns.Keys; }
        }

        public bool HasAnyChange
        {
            get { return _columns.Values.Any(v => v.Modifications.HasChanges || v.Modifications.IsDropped); }
        }

        public ICollection<ColumnModifications> ChangedColumns
        {
            get { return _columns.Values.Where(d => d.Modifications.HasChanges).Select(d => d.Modifications).ToArray(); }
        }

        public ICollection<ColumnModifications> DroppedColumns
        {
            get { return _columns.Values.Where(d => d.Modifications.IsDropped).Select(d => d.Modifications).ToArray(); }
        }

        public ICollection<ColumnModifications> Renames
        {
            get
            {
                return
                    _columns.Where(d => d.Value.Modifications.NewName != null)
                            .Select(d => d.Value.Modifications)
                            .ToArray();
            }
        }

        public string TableName
        {
            get { return _table; }
        }

        #region Implementation of IEnumerable

        public IEnumerator<ColumnModifications> GetEnumerator()
        {
            return _columns.Values.Select(d => d.Modifications).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}