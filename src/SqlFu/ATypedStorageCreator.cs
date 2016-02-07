using CavemanTools.Model.Persistence;
using SqlFu.Builders;
using SqlFu.Builders.CreateTable;

namespace SqlFu
{
    public abstract class ATypedStorageCreator<T> : ICreateStorage
    {
        protected readonly IDbFactory _db;

        protected ATypedStorageCreator(IDbFactory db)
        {
            _db = db;
        }

        protected TableExistsAction ActionIfTableExists = TableExistsAction.Ignore;

        protected abstract void Configure(IConfigureTable<T> cfg);

        public void Create()
        {
            _db.Do(db =>
            {
                db.CreateTableFrom<T>(table =>
                {
                    table.IfExists(ActionIfTableExists);
                    Configure(table);
                });
            });
        }
    }
}