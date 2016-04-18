using System;
using System.Linq.Expressions;
using SqlFu.Configuration;

namespace SqlFu.Builders.CreateTable
{
    public interface IConfigureForeignKeys<T, TParent>
    {
        IConfigureForeignKeys<T, TParent> Columns(params Expression<Func<T, object>>[] columns);
        IConfigureForeignKeys<T, TParent> Reference(params Expression<Func<TParent, object>>[] columns);        
        IConfigureForeignKeys<T, TParent> OnUpdate(ForeignKeyRelationCascade action);        
        IConfigureForeignKeys<T, TParent> OnDelete(ForeignKeyRelationCascade action);        
    }
}