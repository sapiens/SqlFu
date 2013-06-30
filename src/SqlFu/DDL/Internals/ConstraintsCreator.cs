using System;

namespace SqlFu.DDL.Internals
{
    internal class ConstraintsCreator : ICreateConstraints, IDefineSpecificConstraintsOptions
    {
        private readonly ConstraintsCollection _schema;

        public ConstraintsCreator(ConstraintsCollection schema)
        {
            _schema = schema;
        }

        #region Implementation of ISupportSpecificConstraintsOptions

        private DbEngine? _engine;

        protected ConstraintsCollection Schema
        {
            get { return _schema; }
        }

        public IDefineSpecificConstraintsOptions IfDatabaseIs(DbEngine engine)
        {
            _engine = engine;
            return this;
        }

        public IDefineSpecificConstraintsOptions PrimaryKeyOptions(params DbSpecificOption[] options)
        {
            foreach (var opt in options) Schema.PrimaryKey.Options.Add(opt);
            return this;
        }

        public IDefineSpecificConstraintsOptions UniqueOptions(string keyName, params DbSpecificOption[] options)
        {
            Schema.SetUniqueOptions(keyName, options);
            return this;
        }

        public IDefineSpecificConstraintsOptions AddConstraint(string definition)
        {
            definition.MustNotBeEmpty();
            Schema.AddSpecific(_engine.Value, definition);
            return this;
        }

        public IDefineSpecificConstraintsOptions Redefine(string definition)
        {
            definition.MustNotBeEmpty();
            if (_current != null)
            {
                _current.Redefine(_engine.Value, definition);
            }
            return this;
        }

        #endregion

        #region Implementation of ICreateConstraints

        public ICreateConstraints AddPrimaryKeyOn(string columnsNames, string keyName = null)
        {
            _current = Schema.SetPrimaryKey(columnsNames, keyName);
            return this;
        }

        private ConstraintDefinition _current;

        public ICreateConstraints AddUniqueConstraintOn(string columnsNames, string constraintName = null)
        {
            _current = Schema.AddUnique(columnsNames, constraintName);
            return this;
        }

        public ICreateConstraints AddForeignKeyOn(string columnNames, string parentTable, string parentColumns,
                                                  ForeignKeyRelationCascade onUpdate =
                                                      ForeignKeyRelationCascade.NoAction,
                                                  ForeignKeyRelationCascade onDelete =
                                                      ForeignKeyRelationCascade.NoAction, string keyName = null)
        {
            _current = Schema.AddForeignKey(columnNames, parentTable, parentColumns, onUpdate, onDelete, keyName);
            return this;
        }

        public ICreateConstraints AddCheck(string expression, string constraintName)
        {
            _current = Schema.AddCheck(expression, constraintName);
            return this;
        }

        public IDefineSpecificConstraintsOptions this[string name]
        {
            get
            {
                name.MustNotBeEmpty();
                _current = Schema.Checks.Find(d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                if (_current == null)
                {
                    _current = Schema.Uniques.Find(d => d.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                    if (_current == null)
                    {
                        if (Schema.PrimaryKey.Name == name)
                        {
                            _current = Schema.PrimaryKey;
                        }
                        else
                        {
                            throw new ArgumentException("Constraint not found", name);
                        }
                    }
                }
                return this;
            }
        }

        #endregion
    }

    internal class ConstraintsEditor : ConstraintsCreator, IModifyConstraints,
                                       ISupportOptionsForDrop<IModifyConstraints>
    {
        private readonly IModifyTable _parent;

        public ConstraintsEditor(ConstraintsCollection schema, IModifyTable parent) : base(schema)
        {
            _parent = parent;
        }

        #region Implementation of IModifyConstraints

        private DroppedSchemaItem _current;

        public ISupportOptionsForDrop<IModifyConstraints> Drop(string name)
        {
            _current = Schema.Dropped.Add(name);
            return this;
        }

        public ISupportOptionsForDrop<IModifyConstraints> DropPrimaryKey()
        {
            _current = Schema.Dropped.AddPrimaryKey();
            return this;
        }

        #endregion

        #region Implementation of ISupportOptionsForDrop<IModifyConstraints>

        public IModifyConstraints WithOptionsForDatabase(DbEngine engine, params DbSpecificOption[] options)
        {
            _current.Options.AddRange(options);
            return this;
        }

        public IModifyTable WithoutOptions()
        {
            _current = null;
            return _parent;
        }

        #endregion
    }
}