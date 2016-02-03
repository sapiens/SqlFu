using System;
using SqlFu.Builders.Crud;
using SqlFu.Configuration;
using SqlFu.Providers;

namespace SqlFu.Builders
{
    public class SqlFrom : IBuildQueryFrom
    {
        private readonly IDbProvider _provider;
        private readonly ITableInfoFactory _infos;

        public SqlFrom(IDbProvider provider,ITableInfoFactory infos)
        {
            _provider = provider;
            _infos = infos;
        }

        public IWhere<T> From<T>(Action<IHelperOptions> cfg = null)
        {
            var data = new HelperOptions();
            cfg?.Invoke(data);
            return new SimpleSqlBuilder<T>(data,_provider,_infos);
        }

        public IWhere<T> FromAnonymous<T>(T anon, Action<IHelperOptions> options) where T : class
        {
            var data=new HelperOptions();
            options.MustNotBeNull();
            options(data);
            return new SimpleSqlBuilder<T>(data, _provider, _infos);
        }
    }
}