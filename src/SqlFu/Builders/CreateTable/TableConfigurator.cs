using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.CreateTable
{
    internal class TableConfigurator<T> : IConfigureTable<T>
    {
        private readonly IDbProvider _provider;

        TableCreationData _data=new TableCreationData(typeof(T));

        public TableConfigurator(IDbProvider provider)
        {
            _provider = provider;
            //_data.Name = typeof(T).Name;
            AddDefaultColumns();
        }

        public TableCreationData Data => _data;

        private void AddDefaultColumns()
        {
            _data.Columns.AddRange(typeof (T).GetMembers(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.Is<PropertyInfo>() || m.Is<FieldInfo>())
                .Select(GetColumn));
        }

        private ColumnDefinition GetColumn(MemberInfo info)
        {
            var col = new ColumnDefinition();
            col.PropertyName = info.Name;
            var type=col.Type= info.GetMemberType();
            col.DbType = _provider.GetColumnType(type);
            var notnull = new[] { typeof(string), typeof(byte[]) };
            if ((type.IsClass() && !notnull.Contains(type))|| type.IsNullable()) col.IsNull = true;
            return col;
        }

        public IConfigureTable<T> TableName(string name, string schema = null)
        {
            _data.TableName=new TableName(name,schema);            
            return this;
        }

       

        public IConfigureTable<T> Column(Expression<Func<T, object>> column, Action<IConfigureColumn> cfg)
        {
            column.MustNotBeNull();
            cfg.MustNotBeNull();
            var name = column.GetPropertyName();
            var info = _data.Column(name);
            cfg(info);
          
            return this;
        }

        public IConfigureTable<T> AddColumn(string definition)
        {
            
            definition.MustNotBeEmpty();
            _data.Columns.Add(new ColumnDefinition() {Definition = definition});
            return this;
        }

        public IConfigureTable<T> Index(Action<IConfigureIndex<T>> cfg)
        {
            var idx=new IndexDefinition<T>();
            cfg(idx);
            _data.Indexes.Add(idx);
            return this;
            
        }

       
        public IConfigureTable<T> PrimaryKey(Action<IConfigurePrimaryKey<T>> cfg)
        {
            var data=new PKData<T>();
            cfg(data);
            _data.PrimaryKey = data;
            return this;
        }

        public IConfigureTable<T> ForeignKeyFrom<TParent>(Action<IConfigureForeignKeys<T, TParent>> cfg, string name = null)
        {
            var data = new ForeignKeyDefinition() {Name = name};
            var f=new FKConfig<T,TParent>(data,SqlFuManager.Config.TableInfoFactory) ;
            cfg(f);
            _data.ForeignKeys.Add(data);
            return this;
        }

        public IConfigureTable<T> IfTableExists(IfTableExists action)
        {
            _data.CreationOptions = action;
            return this;
        }
    }
}