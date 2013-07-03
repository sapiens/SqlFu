using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlFu.DDL.Internals
{
    internal class ConstraintsCollection
    {
        private readonly TableSchema _table;

        public ConstraintsCollection(TableSchema table)
        {
            _table = table;
            Checks = new List<CheckConstraint>();
            Uniques = new List<UniqueKeyConstraint>();
            ForeignKeys = new List<ForeignKeyConstraint>();
            Dropped = new DroppedSchemaItemsCollection(table.Name);
        }

        public ConstraintDefinition SetPrimaryKey(string columnsNames, string keyName = null)
        {
            columnsNames.MustNotBeEmpty();
            if (keyName == null) keyName = "PK_" + _table.TableName.FilterEscape();
            PrimaryKey = new UniqueKeyConstraint {Name = keyName, Columns = columnsNames, IsPrimary = true};
            return PrimaryKey;
        }

        public bool HasConstraints
        {
            get { return Checks.Count > 0 || Uniques.Count > 0 || ForeignKeys.Count > 0 || (PrimaryKey != null); }
        }

        public List<CheckConstraint> Checks { get; private set; }

        public ConstraintDefinition AddCheck(string expression, string constraintName)
        {
            constraintName.MustNotBeEmpty();
            var c = new CheckConstraint {Expression = expression, Name = constraintName};
            Checks.Add(c);
            return c;
        }

        public List<UniqueKeyConstraint> Uniques { get; private set; }

        public ConstraintDefinition AddUnique(string columns, string name = null)
        {
            columns.MustNotBeEmpty();
            if (name == null) name = GenerateIndexName(columns);
            var uc = new UniqueKeyConstraint {Name = name, Columns = columns};
            Uniques.Add(uc);
            return uc;
        }

        public void SetUniqueOptions(string name, params DbSpecificOption[] options)
        {
            var uc = Uniques.Find(u => u.Name == name);
            if (uc != null)
            {
                foreach (var opt in options) uc.Options.Add(opt);
            }
        }

        private string GenerateIndexName(string columns)
        {
            return "UC_" + _table.TableName + "_" + _table.ColumnsToName(columns);
        }

        public UniqueKeyConstraint PrimaryKey { get; private set; }

        private readonly List<Tuple<DbEngine, string>> _added = new List<Tuple<DbEngine, string>>();

        public void AddSpecific(DbEngine engine, string definition)
        {
            definition.MustNotBeEmpty();
            _added.Add(new Tuple<DbEngine, string>(engine, definition));
        }


        public IEnumerable<string> GetSpecificConstraints(DbEngine engine)
        {
            return _added.FindAll(d => d.Item1 == engine).Select(d => d.Item2);
        }

        public List<ForeignKeyConstraint> ForeignKeys { get; private set; }

        public ConstraintDefinition AddForeignKey(string columnNames, string parentTable, string parentColumns,
                                                  ForeignKeyRelationCascade onUpdate =
                                                      ForeignKeyRelationCascade.NoAction,
                                                  ForeignKeyRelationCascade onDelete =
                                                      ForeignKeyRelationCascade.NoAction, string keyName = null)
        {
            columnNames.MustNotBeEmpty();
            parentTable.MustNotBeEmpty();
            parentColumns.MustNotBeEmpty();
            var fk = new ForeignKeyConstraint
                {
                    Columns = columnNames,
                    ParentTable = parentTable,
                    Name = keyName ?? GenerateForeignKeyName(columnNames, parentTable, parentColumns),
                    ParentColumn = parentColumns,
                    OnUpdate = onUpdate,
                    OnDelete = onDelete
                };
            ForeignKeys.Add(fk);
            return fk;
        }

        private string GenerateForeignKeyName(string columns, string parentTable, string parentColumns)
        {
            return "FK_" + _table.TableName + "_" + parentTable + "_" + _table.ColumnsToName(columns);
        }

        public DroppedSchemaItemsCollection Dropped { get; private set; }
    }
}