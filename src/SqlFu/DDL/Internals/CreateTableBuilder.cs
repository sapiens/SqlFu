using System;
using SqlFu.DDL.Generators;

namespace SqlFu.DDL.Internals
{
    internal class CreateTableBuilder:ICreateTable
    {
        private readonly IAccessDb _db;
        private readonly IGenerateDDL _generator;
        TableSchema _table;
        public CreateTableBuilder(IAccessDb db,IGenerateDDL generator,string tableName,IfTableExists option)
        {
            _db = db;
            _generator = generator;
            tableName.MustNotBeEmpty();
            _table= new TableSchema(tableName);
            _columns= new ColumnsCreator(_table);
            _constraints = new ConstraintsCreator(_table.Constraints);
            _indexes = new IndexCreator(_table.Indexes);

            _table.Name = tableName;
            _table.CreationOption = option;                 
            
        }
        ColumnsCreator _columns;
        private ConstraintsCreator _constraints;
        private IndexCreator _indexes;

        public string GetSql()
        {
            return _generator.GenerateCreateTable(_table);
        }

        public void ExecuteDDL()
        {
            if (ContinueIfTableExists())_db.ExecuteCommand(GetSql());            
        }

        bool ContinueIfTableExists()
        {
            var tools = _db.DatabaseTools;
            
            if (tools.TableExists(_table.Name))
            {
                switch (_table.CreationOption)
                {
                    case IfTableExists.DropIt:
                        tools.DropTable(_table.Name);
                        break;
                    case IfTableExists.Throw:
                        throw new TableExistsException(_table.Name);
                    case IfTableExists.Ignore:
                        return false;
                }
            }
            return true;
        }

        public ICreateTable TableOptionsFor(DbEngine engine, params DbSpecificOption[] options)
        {
            _table.Options.AddRange(options);
            return this;
        }

        public IAddColumns Columns
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

       

        #region Inner Classes
        
        #endregion

        
    }
}