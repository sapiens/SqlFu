using System;
using System.Linq.Expressions;

namespace SqlFu.Configuration.Internals
{
    public class ForeignKey : BaseConstraint<IConfigureForeignKey>,IConfigureForeignKey,IConfigureForeignKeyConstraints
    {
        public ForeignKey()
        {
            
        }

        public IConfigureForeignKey Named(string name)
        {
            name.MustNotBeNull();
            Name = name;
            return this;
        }

        IConfigureForeignKeyConstraints IConfigureForeignKeyConstraints.OnUpdate(ForeignKeyRelationCascade option)
        {
            OnUpdate = option;
            return this;
        }

        IConfigureForeignKeyConstraints IConfigureForeignKeyConstraints.OnDelete(ForeignKeyRelationCascade option)
        {
            OnDelete = option;
            return this;
        }

        public IConfigureForeignKeyConstraints References<T>(params Expression<Func<T, object>>[] columns)
        {
            var info = SqlFuManager.Config.TableInfoFactory.GetInfo(typeof (T));
            ReferencedColumns = info.ToColumnsInfo(columns);
            return this;
        }

        public ColumnInfo[] ReferencedColumns { get; set; }
        public ForeignKeyRelationCascade OnUpdate { get; set; }
        public ForeignKeyRelationCascade OnDelete { get; set; }
    }
}