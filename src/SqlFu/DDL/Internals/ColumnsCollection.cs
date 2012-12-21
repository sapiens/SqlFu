using System;
using System.Collections.Generic;

namespace SqlFu.DDL.Internals
{
    internal class ColumnsCollection : List<ColumnDefinition>
    {
        public void AddColumn(ColumnDefinition col)
        {
            if (this.Exists(c => c.Name == col.Name))
                throw new InvalidOperationException("Column '" + col.Name + "' already exists");
            Add(col);
            if (!IsIdentityDefined)
            {
                IsIdentityDefined = col.IsIdentity;
            }
        }

        public ColumnDefinition this[string name]
        {
            get { return Find(d => d.Name == name); }
        }

        public bool IsIdentityDefined { get; private set; }
    }
}