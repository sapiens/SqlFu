using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Configuration
{
    public abstract class APocoToTableMapping<T> : IConfigureTableInfo where T:class 
    {
        protected APocoToTableMapping()
        {
            Info = new TableInfo(typeof(T),SqlFuManager.Config.Converters);
               
        }

        protected APocoToTableMapping(TableInfo info)
        {
            Info = info;
        }

        protected void TableNameIs(string name,string schema=null)
        {
            name.MustNotBeEmpty();
            Info.Name = name;
            Info.SchemaName = schema;
        }

        protected void TableSchemaIs(string schema)
        {
            schema.MustNotBeEmpty();
            Info.SchemaName = schema;
        }

        protected void Ignore(params Expression<Func<T, object>>[] properties)
        {
           Info.RemoveColumns(properties.Select(d=>d.Body.GetPropertyName()).ToArray());
        
        }

        protected void TableOptionsFor(string providerId, params DbSpecificOption[] options)
        {
            Info.ProviderFeatures[providerId].AddRange(options);
        }

        protected ISetCreationOptions IfTableExists()
        {
            return new SetCreationOption(Info);
        }

        protected IConfigurePrimaryKey PrimaryKeyOn(params Expression<Func<T, object>>[] columns)
        {
            Info.PrimaryKey= new PrimaryKey()
            {
                Columns = Info.ToColumnsInfo(columns)                
            };
            return Info.PrimaryKey;
        }

        protected IConfigureIndex IndexOn(params Expression<Func<T, object>>[] columns)
        {
            var idx = new Index(){Columns = Info.ToColumnsInfo(columns)};
            Info.Indexes.Add(idx);
            return idx;
        }

        //todo overload with expression
        protected void AddCheck(string name, string expression)
        {
            name.MustNotBeEmpty();
            expression.MustNotBeEmpty();
            var check = new Check() {Name = name, Expression = expression};
            Info.Checks.Add(check);
        }

        protected IConfigureColumn Column(Expression<Func<T, object>> property)
        {
            var name = property.Body.GetPropertyName();
            return Info.Columns.First(c => c.PropertyInfo.Name == name);
        }

        protected IConfigureForeignKey ForeignKeyOn(params Expression<Func<T, object>>[] columns)
        {
            var fk = new ForeignKey();
            Info.ForeignKeys.Add(fk);
            fk.Columns = Info.ToColumnsInfo(columns);
            return fk;
        }



        public TableInfo Info { get; private set; }

#region Classes

        class SetCreationOption : ISetCreationOptions
        {
            private readonly TableInfo _info;

            public SetCreationOption(TableInfo info)
            {
                _info = info;
            }

            public void Throw()
            {
                _info.CreationOptions=Internals.IfTableExists.Throw;
            }

            public void DropIt()
            {
                _info.CreationOptions = Internals.IfTableExists.DropIt;
            }

            public void Ignore()
            {
                _info.CreationOptions = Internals.IfTableExists.Ignore;
            }
        }
#endregion
    }
}