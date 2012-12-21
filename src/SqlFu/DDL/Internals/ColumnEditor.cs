using System;
using System.Data;

namespace SqlFu.DDL.Internals
{
    internal class ColumnEditor : IChangeColumn
    {
        private readonly string _columnName;
        private readonly ColumnModifications _data;

        public ColumnEditor(string columnName, string table)
        {
            _columnName = columnName;
            _data = new ColumnModifications(columnName, table);
        }

        public ColumnModifications Modifications
        {
            get { return _data; }
        }

        public string ColumnName
        {
            get { return _columnName; }
        }

        public IChangeColumn SetType(DbType type, string size)
        {
            _data.Type = type;
            _data.Size = size;
            return this;
        }

        public IChangeColumn RenameTo(string name)
        {
            name.MustNotBeEmpty();
            _data.NewName = name;
            return this;
        }

        public IChangeColumn SetSize(string size)
        {
            _data.Size = size;
            return this;
        }

        public IChangeColumn SetCollation(string collation)
        {
            collation.MustNotBeEmpty();
            _data.Collation = collation;
            return this;
        }

        public IChangeColumn SetDefault(string defaultValue)
        {
            _data.DefaultValue = defaultValue;
            _data.DefaultDropped = false;
            return this;
        }

        public IChangeColumn SetNullable(bool isNullable)
        {
            _data.Nullable = isNullable;
            return this;
        }

        public IChangeColumn DropDefault()
        {
            _data.DefaultDropped = true;
            return this;
        }
    }
}