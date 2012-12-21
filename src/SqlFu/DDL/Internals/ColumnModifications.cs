using System;
using System.Data;

namespace SqlFu.DDL.Internals
{
    internal class ColumnModifications
    {
        public ColumnModifications(string name, string table)
        {
            name.MustNotBeEmpty();
            table.MustNotBeEmpty();
            Options = new DbEngineOptions();
            Current = new CurrentColumnSchema();
            Current.Name = name;
            TableName = table;
        }

        public string TableName { get; set; }
        public DbType? Type { get; set; }
        public string NewName { get; set; }
        public string Size { get; set; }
        public string Collation { get; set; }
        public string DefaultValue { get; set; }
        public bool DefaultDropped { get; set; }
        public bool IsDropped { get; set; }
        public bool? Nullable { get; set; }
        internal DbEngineOptions Options { get; private set; }
        public CurrentColumnSchema Current { get; set; }

        /// <summary>
        /// Gets the column's current name. It takes renaming into account 
        /// </summary>
        public string Name
        {
            get { return NewName ?? Current.Name; }
        }

        public bool HasChangedStructure
        {
            get { return Type != null || Collation != null || Nullable != null; }
        }

        public bool HasChanges
        {
            get { return Type != null || Collation != null || Nullable != null || DefaultValue != null || DefaultDropped; }
        }
    }

    public class CurrentColumnSchema
    {
        private string _definition;
        public string Type { get; set; }
        public string Collation { get; set; }
        public string Name { get; set; }
        public string DefaultConstraintName { get; set; }

        public string Definition
        {
            get
            {
                if (_definition.IsNullOrEmpty())
                {
                    return string.Format("{0} {1} {2}", Name, Type, Collation);
                }
                return _definition;
            }
            set { _definition = value; }
        }
    }
}