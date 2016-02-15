using System;
using CavemanTools.Model.Persistence;
using SqlFu.Builders;
using SqlFu.Builders.CreateTable;

namespace SqlFu
{
    public abstract class ATypedStorageCreator<T> : ICreateStorage
    {
        private const string NoSchema = "";
        protected readonly IDbFactory _db;

        protected ATypedStorageCreator(IDbFactory db)
        {
            _db = db;
        }

        public ATypedStorageCreator<T> IfExists(TableExistsAction action)
        {
            HandleExistingTable = action;
            return this;
        }

        private string _name;
        private string _schema;

        public ATypedStorageCreator<T> WithTableName(string name,string schema=NoSchema)
        {
            name.MustNotBeEmpty();        
            _name = name;
            _schema = schema;
            return this;
        }

        protected TableExistsAction HandleExistingTable = TableExistsAction.Ignore;

        protected abstract void Configure(IConfigureTable<T> cfg);

        public void Create()
        {
            _db.HandleTransientErrors(db =>
            {
                db.CreateTableFrom<T>(table =>
                {
                    table
                        .HandleExisting(HandleExistingTable);
                    if (_name.IsNotEmpty()) table.TableName(_name,_schema);
                    Configure(table);
                });
            });
        }
    }
}