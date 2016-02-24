using System;
using System.Collections.Generic;
using System.Data.Common;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
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
        }

        public CustomMappersConfiguration CustomMappers => _customMappers;

        public ConvertersManager Converters => _converters;

        public TableInfoFactory TableInfoFactory => _tableInfoFactory;

        public MapperFactory MapperFactory { get; }

        public void ConfigureTableForPoco<T>(Action<ITableInfo> cfg)
        {
            cfg.MustNotBeNull();
            var table = TableInfoFactory.GetInfo(typeof (T));
            cfg(table);
        }

        ///// <summary>
        ///// Registers mappings for a value object, in order to flatten it (insert/update helpers only) or to restore it.
        ///// For simple conversion of a result to a value object use <see cref="SqlFuConfig.RegisterConverter"/>
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="from">Flattens the value object</param>
        ///// <param name="to">Restores the value object</param>
        //public void MapValueObject<T>(Func<T, object> from, Func<object, T> to = null)
        //{
        //    Converters.MapValueObject(from, to);
        //}

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

        public void AddProfile(IDbProvider provider, string connectionString, string name = "default")
        {
            provider.MustNotBeNull();
            _profiles[name] = new DbAccessProfile() {ConnectionString = connectionString, Name = name, Provider = provider};

        }

        public DbAccessProfile GetProfile(string name="default") => _profiles[name];

        public bool HasNoProfiles => !_profiles.HasItems();

    }
}