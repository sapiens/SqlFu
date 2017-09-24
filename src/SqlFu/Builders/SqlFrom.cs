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
            var data = new HelperOptions(_infos.GetInfo(typeof(T)));
            cfg?.Invoke(data);
            return SqlBuilder<T>(data);
        }

        IWhere<T> SqlBuilder<T>(HelperOptions options)=> new SimpleSqlBuilder<T>(
                options, _provider, new ExpressionSqlGenerator(_provider.ExpressionsHelper,_infos,_provider),_infos);

        public IWhere<T> FromAnonymous<T>(T anon, TableName tableName) where T : class
        {
            var data=new HelperOptions(_infos.GetInfo(typeof(T)));
            tableName.MustNotBeNull();
            data.TableName = tableName;
            return SqlBuilder<T>(data);
        }
    }
}