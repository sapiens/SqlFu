using System;

namespace SqlFu.DDL
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PrimaryKeyAttribute : Attribute
    {
        public PrimaryKeyAttribute(params string[] columns)
        {
            columns.MustHaveValues();
            Columns = columns;
            AutoIncrement = true;
        }

        public string Name { get; set; }
        public string[] Columns { get; private set; }

        /// <summary>
        /// Default is true
        /// </summary>
        public bool AutoIncrement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class IndexAttribute : Attribute
    {
        public IndexAttribute(params string[] columns)
        {
            columns.MustHaveValues();
            Columns = columns;
        }

        public string Name { get; set; }
        public string[] Columns { get; private set; }
        public bool IsUnique { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string parentTable, string parentColumn)
        {
            parentColumn.MustNotBeEmpty();
            parentTable.MustNotBeEmpty();
            ParentColumn = parentColumn;
            ParentTable = parentTable;
        }

        public string KeyName { get; set; }
        public string ParentTable { get; private set; }
        public string ParentColumn { get; private set; }
        public ForeignKeyRelationCascade OnUpdate { get; set; }

        public ForeignKeyRelationCascade OnDelete { get; set; }
    }

    /// <summary>
    /// Tell SqlFu to use the provided definition when generating the column for a specific db engine
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RedefineForAttribute : Attribute
    {
        public RedefineForAttribute(DbEngine engine, string definition)
        {
            if (engine == DbEngine.None) throw new ArgumentException("I need a database engine");
            definition.MustNotBeEmpty();
            Database = engine;
            Definition = definition;
        }

        public DbEngine Database { get; private set; }
        public string Definition { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnOptionsAttribute : Attribute
    {
        internal static ColumnOptionsAttribute Default = new ColumnOptionsAttribute();

        /// <summary>
        /// Skip this column when generating table
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Default is false
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Null means maximum size
        /// </summary>
        public string Size { get; set; }

        public string DefaultValue { get; set; }
    }
}