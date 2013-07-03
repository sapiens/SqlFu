using System;
using System.Data.Common;
using SqlFu.DDL.Generators;

namespace SqlFu.DDL.Internals
{
    internal class ModifyTableBuilder : IModifyTable
    {
        private readonly DbConnection _db;
        private readonly IGenerateDDL _generator;
        private readonly TableSchema _table;
        private readonly ColumnsEditor _columns;
        private readonly ConstraintsEditor _constraints;
        private readonly IndexEditor _indexes;

        public ModifyTableBuilder(DbConnection db, IGenerateDDL generator, string name)
        {
            _db = db;
            _generator = generator;
            name.MustNotBeEmpty();
            _table = new TableSchema(name);

            _columns = new ColumnsEditor(Table, this);
            _constraints = new ConstraintsEditor(Table.Constraints, this);
            _indexes = new IndexEditor(Table.Indexes, this);
        }

        public string GetSql()
        {
            return _generator.GenerateAlterTable(_table);
        }

        public void ExecuteDDL()
        {
            using (var t = _db.BeginTransaction())
            {
                _db.ExecuteCommand(GetSql());
                t.Commit();
            }
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