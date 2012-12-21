using System;
using System.Collections.Generic;

namespace SqlFu.DDL.Internals
{
    internal class DroppedSchemaItemsCollection : List<DroppedSchemaItem>
    {
        private readonly string _tableName;

        public DroppedSchemaItemsCollection(string tableName)
        {
            tableName.MustNotBeEmpty();
            _tableName = tableName;
        }

        public DroppedSchemaItem AddPrimaryKey()
        {
            var item = new DroppedSchemaItem(null, _tableName) {IsPrimaryKey = true};
            Add(item);
            return item;
        }

        public DroppedSchemaItem Add(string name)
        {
            name.MustNotBeEmpty();
            DroppedSchemaItem item = Find(d => d.Name == name);
            if (item == null)
            {
                item = new DroppedSchemaItem(name, _tableName);
                Add(item);
            }
            return item;
        }
    }
}