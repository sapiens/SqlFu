using System;
using System.Linq.Expressions;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders.CreateTable
{
    public interface IConfigureTable<T>
    {
        IConfigureTable<T> TableName(string name, string schema = "");
        IConfigureTable<T> Column(Expression<Func<T,object>> column,Action<IConfigureColumn> cfg);
        IConfigureTable<T> AddColumn(string definition);
        IConfigureTable<T> Index(Action<IConfigureIndex<T>> cfg);
        IConfigureTable<T> PrimaryKey(Action<IConfigurePrimaryKey<T>> cfg);
        IConfigureTable<T> ForeignKeyFrom<TParent>(Action<IConfigureForeignKeys<T,TParent>> cfg,string name=null);
        IConfigureTable<T> IfTableExists(IfTableExists action);


    }
}