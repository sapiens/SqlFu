using System;
using SqlFu.Builders.Crud;
using SqlFu.Builders.Expressions;
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
            return SqlBuilder<T>(data);
        }

        IWhere<T> SqlBuilder<T>(HelperOptions options)=> new SimpleSqlBuilder<T>(
                options, _provider, _infos.GetInfo(typeof(T))
                , new ExpressionWriterHelper(_infos, _provider).CreateExpressionWriter());

        public IWhere<T> FromAnonymous<T>(T anon, Action<IHelperOptions> options) where T : class
        {
            var data=new HelperOptions();
            options.MustNotBeNull();
            options(data);
            return SqlBuilder<T>(data);
        }
    }
}