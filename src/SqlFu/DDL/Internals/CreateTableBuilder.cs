using System;
using System.Data.Common;
using SqlFu.DDL.Generators;

namespace SqlFu.DDL.Internals
{
    internal class CreateTableBuilder : ICreateTable
    {
        private readonly DbConnection _db;
        private readonly IGenerateDDL _generator;
        private readonly TableSchema _table;

        public CreateTableBuilder(DbConnection db, IGenerateDDL generator, TableSchema schema)
        {
            _db = db;
            _generator = generator;
            _table = schema;
            _columns = new ColumnsCreator(Table);
            _constraints = new ConstraintsCreator(Table.Constraints);
            _indexes = new IndexCreator(Table.Indexes);
        }

        public CreateTableBuilder(DbConnection db, IGenerateDDL generator, string tableName, IfTableExists option)
        {
            _db = db;
            _generator = generator;
            tableName.MustNotBeEmpty();
            _table = new TableSchema(tableName);
            _columns = new ColumnsCreator(Table);
            _constraints = new ConstraintsCreator(Table.Constraints);
            _indexes = new IndexCreator(Table.Indexes);

            Table.Name = tableName;
            Table.CreationOption = option;
        }

        private readonly ColumnsCreator _columns;
        private readonly ConstraintsCreator _constraints;
        private readonly IndexCreator _indexes;

        public string GetSql()
        {
            return _generator.GenerateCreateTable(Table);
        }

        public void ExecuteDDL()
        {
            if (ContinueIfTableExists()) _db.ExecuteCommand(GetSql());
        }

        private bool ContinueIfTableExists()
        {
            var tools = _db.DatabaseTools();

            if (tools.TableExists(Table.Name))
            {
                switch (Table.CreationOption)
                {
                    case IfTableExists.DropIt:
                        tools.DropTable(Table.Name);
                        break;
                    case IfTableExists.Throw:
                        throw new TableExistsException(Table.Name);
                    case IfTableExists.Ignore:
                        return false;
                }
            }
            return true;
        }

        public ICreateTable TableOptionsFor(DbEngine engine, params DbSpecificOption[] options)
        {
            Table.Options.AddRange(options);
            return this;
        }

        public ICreateColumns Columns
        {
            get { return _columns; }
        }

        public ICreateConstraints Constraints
        {
            get { return _constraints; }
        }

        public ICreateIndexes Indexes
        {
            get { return _indexes; }
        }

        internal TableSchema Table
        {
            get { return _table; }
        }

        #region Inner Classes

        #endregion
    }
}