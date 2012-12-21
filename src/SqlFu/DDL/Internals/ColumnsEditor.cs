using System;

namespace SqlFu.DDL.Internals
{
    internal class ColumnsEditor : ColumnsCreator, IModifyColumns, ISupportOptionsForDrop<IModifyColumns>
    {
        private readonly TableSchema _table;
        private readonly IModifyTable _parent;

        public ColumnsEditor(TableSchema table, IModifyTable parent) : base(table)
        {
            _table = table;
            _parent = parent;
        }

        public IModifyColumns RenameColumn(string currentName, string newName)
        {
            currentName.MustNotBeEmpty();
            newName.MustNotBeEmpty();
            Change(currentName).RenameTo(newName);
            return this;
        }

        public IChangeColumn Change(string name)
        {
            return _table.ModifiedColumns[name];
        }

        private ColumnModifications _dropped;

        public ISupportOptionsForDrop<IModifyColumns> Drop(string name)
        {
            _dropped = _table.ModifiedColumns[name].Modifications;
            _dropped.IsDropped = true;
            return this;
        }

        #region Implementation of ISupportOptionsForDrop<IModifyColumns>

        public IModifyColumns WithOptionsForDatabase(DbEngine engine, params DbSpecificOption[] options)
        {
            _dropped.Options.AddRange(options);
            return this;
        }

        public IModifyTable WithoutOptions()
        {
            _dropped = null;
            return _parent;
        }

        #endregion
    }
}