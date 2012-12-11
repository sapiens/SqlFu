using System;
using System.Data;
using SqlFu.DDL.Generators;

namespace SqlFu.DDL.Internals
{
    internal class ModifyTableBuilder:IModifyTable
    {
        private readonly IAccessDb _db;
        private readonly IGenerateDDL _generator;
        private TableSchema _table;
        private ColumnsEditor _columns;
        private ConstraintsEditor _constraints;
        private IndexEditor _indexes;

        public ModifyTableBuilder(IAccessDb db,IGenerateDDL generator,string name)
        {
            _db = db;
            _generator = generator;
            name.MustNotBeEmpty();
            _table = new TableSchema(name);
            
            _columns = new ColumnsEditor(Table,this);
            _constraints = new ConstraintsEditor(Table.Constraints,this);
            _indexes = new IndexEditor(Table.Indexes,this);                        
        }
        
        public string GetSql()
        {
            return _generator.GenerateAlterTable(_table);
        }

        public void ExecuteDDL()
        {
            _db.ExecuteCommand(GetSql());
        }

        //public ISupportSpecificTableOptions SetOptionsFor(DbEngine engine, params string[] options)
        //{
        //    Table.Options.SetOptions(engine,options);
        //    return this;
        //}

        public IModifyColumns Columns
        {
            get { return _columns; }
        }

        public IModifyConstraints Constraints
        {
            get { return _constraints; }
        }

        public IModifyIndexes Indexes
        {
            get { return _indexes; }
        }

        public TableSchema Table
        {
            get { return _table; }
        }

        ///// <summary>
        ///// Don't forget to drop indexes, foreign keys and other constraints dependant on the column
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //public IModifyTable DropColumn(string name)
        //{
        //    name.MustNotBeEmpty();
        //    Table.ModifiedColumns[name].Modifications.IsDropped = true;
        //    return this;
        //}

        //public IModifyTable DropConstraint(string name)
        //{
        //    name.MustNotBeEmpty();
        //    Table.Constraints.Dropped.Add(name);
        //    return this;
        //}

        //public IModifyTable DropIndex(string idxName)
        //{
        //    idxName.MustNotBeEmpty();
        //    Table.DroppedIndexes.Add(idxName);
        //    return this;
        //}

        #region Implementation of ISupportSpecificTableOptions<IModifyTable>

        public IModifyTable TableOptionsFor(DbEngine engine, params DbSpecificOption[] options)
        {
            _table.Options.AddRange(options);
            return this;
        }

        #endregion
    }
}