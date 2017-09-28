using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Executors.Resilience;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;

namespace SqlFu
{
    public class SqlFuConfig
    {
        private readonly CustomMappersConfiguration _customMappers = new CustomMappersConfiguration();
        private readonly ConvertersManager _converters = new ConvertersManager();
        private readonly TableInfoFactory _tableInfoFactory;

        public SqlFuConfig()
        {
            _tableInfoFactory = new TableInfoFactory(_converters);
            MapperFactory = new MapperFactory(_customMappers, _tableInfoFactory, _converters);
            var ab = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("SqlFuDyn"),
                AssemblyBuilderAccess.Run);
            _module = ab.DefineDynamicModule("Factories"); 
        }

        public CustomMappersConfiguration CustomMappers => _customMappers;

        public ConvertersManager Converters => _converters;

        public TableInfoFactory TableInfoFactory => _tableInfoFactory;

        public MapperFactory MapperFactory { get; }

        /// <summary>
        /// Set/Get factory for transient errors strategy
        /// </summary>
        public Func<IRetryOnTransientErrorsStrategy> TransientErrorsStrategyFactory { get; set; } =
            () => new DefaultTransientErrorsStrategy();


        public void ConfigureDefaultTransientResilience(Action<IConfigureDefaultTransientErrorsStrategy> config)
        {
            TransientErrorsStrategyFactory = () =>
            {
                var s = new DefaultTransientErrorsStrategy();
                config(s);
                return s;
            };
        }


      public void ConfigureTableForPoco<T>(Action<ITableInfo<T>> cfg)
        {
            cfg.MustNotBeNull();
            var table = TableInfoFactory.GetInfo(typeof (T));
            cfg(new TableInfo<T>(table));                  
        }

        public void RegisterConverter<T>(Func<object, T> converter)
        {
            Converters.RegisterConverter(converter);
        }

        public void AddNamingConvention(Func<Type, bool> match, Func<Type, TableName> convention)
        {
            TableInfoFactory.AddNamingConvention(match, convention);
        }

       
        #region Events

        private Action<DbCommand, DbException> _onException = (s, e) => { };

        public Action<DbCommand, DbException> OnException
        {
            get { return _onException; }
            set
            {
                value.MustNotBeNull();
                _onException = value;
            }
        }


        private Action<DbCommand> _onCmd = c => { };

        /// <summary>
        /// Invoked after the command has been executed  
        /// </summary>
        public Action<DbCommand> OnCommand
        {
            get { return _onCmd; }
            set
            {
                value.MustNotBeNull();
                _onCmd = value;
            }
        }


        private Action<DbConnection> _onCloseConex = c => { };

        public Action<DbConnection> OnCloseConnection
        {
            get { return _onCloseConex; }
            set
            {
                value.MustNotBeNull();
                _onCloseConex = value;
            }
        }

        private Action<DbConnection> _onOpenConex = c => { };
      
        public Action<DbConnection> OnOpenConnection
        {
            get { return _onOpenConex; }
            set
            {
                value.MustNotBeNull();
                _onOpenConex = value;
            }
        }


        #endregion



        private Dictionary<string, DbAccessProfile> _profiles = new Dictionary<string, DbAccessProfile>();
        private ModuleBuilder _module;


        public void AddProfile(IDbProvider provider, string connectionString, string name = "default")
        {
            provider.MustNotBeNull();
            _profiles[name] = new DbAccessProfile() {ConnectionString = connectionString, Name = name, Provider = provider};
        }
        public void AddProfile<T>(IDbProvider provider, string connectionString) where T:IDbFactory
        {
           _profiles[typeof(T).Name] = CreateProfile<T>(provider,connectionString);
        }

        public T CreateFactory<T>(IDbProvider provider, string connectionString) where T : IDbFactory
            => (T)CreateProfile<T>(provider, connectionString).Factory;

        private DbAccessProfile CreateProfile<T>(IDbProvider provider, string connectionString) where T : IDbFactory
        {
            provider.MustNotBeNull();
            var type = typeof(T);
            type.GetTypeInfo().IsInterface.MustBe(true);
            var name = type.Name;
            var profile = new DbAccessProfile() { ConnectionString = connectionString, Name = name, Provider = provider, Factory = CreateFactory<T>() };
            profile.Factory.CastAs<DbFactory>().Assign(profile);
            return profile;
        }


        /// <summary>
        /// Used to identify a db profile associated with a db interface
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GetProfileNameFor<T>() where T : IDbFactory => typeof(T).Name;

        T CreateFactory<T>()
        {
            var tp = typeof(T);
            
            var fact=_module.DefineType(tp.Name + "_class_"+Guid.NewGuid(), TypeAttributes.Class, typeof(DbFactory), new[] {tp});
            return (T) Activator.CreateInstance(fact.CreateTypeInfo().AsType());

        }

        public DbAccessProfile GetProfile<T>() where T : IDbFactory => GetProfile(GetProfileNameFor<T>());
        public DbAccessProfile GetProfile(string name="default") => _profiles[name];

        public bool HasNoProfiles => !_profiles.HasItems();

    }
}