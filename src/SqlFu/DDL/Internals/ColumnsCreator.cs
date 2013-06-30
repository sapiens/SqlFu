using System;
using System.Collections.Generic;
using System.Data;

namespace SqlFu.DDL.Internals
{
    internal class ColumnsCreator : IDefineSpecificColumnsOptions, IConfigureColumns, ICreateColumns
    {
        private readonly TableSchema _table;

        public ColumnsCreator(TableSchema table)
        {
            _table = table;
        }

        private DbEngine? _curentEngine;

        public IDefineSpecificColumnsOptions IfDatabaseIs(DbEngine engine)
        {
            _curentEngine = engine;
            return this;
        }

        private ColumnDefinition _currentColumn;

        public IConfigureColumns Add(string name, DbType type, string size = "", bool isNullable = true,
                                     string defaultValue = "", string collation = "", bool autoIncrement = false)
        {
            var col = new ColumnDefinition
                {
                    Name = name,
                    Type = type,
                    Size = size,
                    DefaultValue = defaultValue,
                    IsNullable = isNullable,
                    IsIdentity = autoIncrement,
                    Collate = collation
                };
            _currentColumn = col;
            _table.Columns.AddColumn(col);
            return this;
        }

        public IDefineSpecificColumnsOptions RedefineColumnAs(string definition)
        {
            definition.MustNotBeEmpty();
            _currentColumn.Redefine(_curentEngine.Value, definition);
            return this;
        }

        public IDefineSpecificColumnsOptions SetColumnOptions(params DbSpecificOption[] options)
        {
            _currentColumn.Options.AddRange(options);
            return this;
        }

        #region Implementation of IConfigureColumns

        public IConfigureColumns AsPrimaryKey(string keyName = null)
        {
            _table.Constraints.SetPrimaryKey(_currentColumn.Name, keyName);
            return this;
        }

        public IConfigureColumns AsUnique(string name = null)
        {
            _table.Constraints.AddUnique(_currentColumn.Name, name);
            return this;
        }

        public IConfigureColumns WithCheck(string expression, string constraintName)
        {
            _table.Constraints.AddCheck(expression, constraintName);
            return this;
        }

        public IConfigureColumns WithIndex(string idxName = null, bool unique = false)
        {
            _table.Indexes.AddIndex(_currentColumn.Name, unique, idxName);
            return this;
        }

        public IConfigureColumns IsForeignKeyFrom(string parentTable, string parentColumns,
                                                  ForeignKeyRelationCascade onUpdate =
                                                      ForeignKeyRelationCascade.NoAction,
                                                  ForeignKeyRelationCascade onDelete =
                                                      ForeignKeyRelationCascade.NoAction, string keyName = null)
        {
            _table.Constraints.AddForeignKey(_currentColumn.Name, parentTable, parentColumns, onUpdate, onDelete,
                                             keyName);
            return this;
        }

        #endregion

        public ISupportSpecificColumnsOptions this[string name]
        {
            get
            {
                var col = _table.Columns[name];
                if (col != null)
                {
                    _currentColumn = col;
                    return this;
                }
                throw new KeyNotFoundException(string.Format(
                    "No column with  the name '{0}' was defined in the builder", name));
            }
        }
    }
}