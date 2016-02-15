using CavemanTools.Model.Persistence;

namespace SqlFu
{
    public abstract class AStorageCreator:ICreateStorage
    {
        private readonly IDbFactory _db;


        protected AStorageCreator(IDbFactory db)
        {
            _db = db;
        }

        protected abstract string ColumnsSql { get; }
        protected string OtherSql { get; } = "";
        protected abstract string TableName { get; }


        public void Create()
        {
            var sql = "create table " + TableName + "( " + ColumnsSql + ");" + (OtherSql ?? "");
            _db.HandleTransientErrors(d => d.Execute(c=>c.Sql(sql,TableName)));
        }
    }
}