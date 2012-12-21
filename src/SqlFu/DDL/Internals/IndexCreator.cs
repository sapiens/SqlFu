using System;

namespace SqlFu.DDL.Internals
{
    internal class IndexCreator : ICreateIndexes, IDefineSpecificIndexOptions
    {
        private readonly IndexCollection _schema;

        public IndexCreator(IndexCollection schema)
        {
            _schema = schema;
        }

        #region Implementation of ISupportSpecificIndexOptions

        private DbEngine? _engine;

        protected IndexCollection Schema
        {
            get { return _schema; }
        }

        public IDefineSpecificIndexOptions IfDatabaseIs(DbEngine engine)
        {
            _engine = engine;
            return this;
        }

        private IndexDefinition _idx;

        public ISupportSpecificIndexOptions AddIndexOn(string columnsNames, bool isUnique, string indexName = null)
        {
            _idx = Schema.AddIndex(columnsNames, isUnique, indexName);
            return this;
        }

        public ISupportSpecificIndexOptions this[string idxName]
        {
            get
            {
                _idx = Schema[idxName];
                if (_idx == null)
                    throw new InvalidOperationException(string.Format("Index is not defined '{0}'", idxName));
                return this;
            }
        }

        #endregion

        #region Implementation of IDefineSpecificIndexOptions

        public IDefineSpecificIndexOptions SetOptions(params DbSpecificOption[] options)
        {
            _idx.Options.AddRange(options);
            return this;
        }

        public IDefineSpecificIndexOptions RedefineAs(string definition)
        {
            _idx.Redefine(_engine.Value, definition);
            return this;
        }

        public IDefineSpecificIndexOptions AddCustom(string definition)
        {
            if (_engine == null)
            {
                throw new InvalidOperationException("No database engine is selected. Please use the other overload");
            }
            Schema.AddSpecific(_engine.Value, definition);
            return this;
        }

        public IDefineSpecificIndexOptions AddCustom(DbEngine engine, string definition)
        {
            Schema.AddSpecific(engine, definition);
            return this;
        }

        #endregion
    }

    internal class IndexEditor : IndexCreator, IModifyIndexes, ISupportOptionsForDrop<IModifyIndexes>
    {
        private readonly IModifyTable _parent;

        public IndexEditor(IndexCollection schema, IModifyTable parent) : base(schema)
        {
            _parent = parent;
        }

        private DroppedSchemaItem _current;

        #region Implementation of IModifyIndexes

        public ISupportOptionsForDrop<IModifyIndexes> Drop(string idxName)
        {
            _current = Schema.Dropped.Add(idxName);
            return this;
        }

        #endregion

        #region Implementation of ISupportOptionsForDrop<IModifyIndexes>

        public IModifyIndexes WithOptionsForDatabase(DbEngine engine, params DbSpecificOption[] options)
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