using System;
using System.Linq.Expressions;

namespace SqlFu.Configuration
{
    public interface IConfigureForeignKey:IConfigureProviderOptions<IConfigureForeignKey>
    {
        IConfigureForeignKey Named(string name);
        IConfigureForeignKeyConstraints References<T>(params Expression<Func<T, object>>[] columns);

    }
}