using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders.CreateTable
{
    internal class FKConfig<T,TParent> : IConfigureForeignKeys<T, TParent>
    {
        private readonly ForeignKeyDefinition _data;
        private readonly ITableInfoFactory _infos;

        public FKConfig(ForeignKeyDefinition data,ITableInfoFactory infos)
        {
            _data = data;
            _infos = infos;
        }

        public IConfigureForeignKeys<T, TParent> Columns(params Expression<Func<T, object>>[] columns)
        {
            _data.Columns = columns.GetNames().ToArray();
            return this;
        }

        public IConfigureForeignKeys<T, TParent> Reference(params Expression<Func<TParent, object>>[] columns)
        {
            var info = _infos.GetInfo(typeof (TParent));
            _data.ParentTable = info.Table;
            _data.ParentColumns = columns.GetNames().ToArray();
            return this;
        }

        public IConfigureForeignKeys<T, TParent> OnUpdate(ForeignKeyRelationCascade action)
        {
            _data.OnUpdate = action;
            return this;
        }

        public IConfigureForeignKeys<T, TParent> OnDelete(ForeignKeyRelationCascade action)
        {
            _data.OnDelete = action;
            return this;
        }
    }
}