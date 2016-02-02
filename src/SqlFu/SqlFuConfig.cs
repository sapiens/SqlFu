using System;
using System.Collections.Generic;

using System.Data.Common;
using SqlFu.Builders.CreateTable;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Mapping.Internals;
using SqlFu.Providers;

namespace SqlFu
{
    public class SqlFuConfig
    {
        private CustomMappers _customMappers = new CustomMappers();
        private ConvertersManager _converters = new ConvertersManager();
        private TableInfoFactory _tableInfoFactory;
        private MapperFactory _mapperFactory;

        public SqlFuConfig()
        {
            _tableInfoFactory = new TableInfoFactory(_converters);

            _mapperFactory = new MapperFactory(_customMappers, _tableInfoFactory, _converters);
        }

        public CustomMappers CustomMappers
        {
            get { return _customMappers; }
        }

        public ConvertersManager Converters
        {
            get { return _converters; }
        }

        public TableInfoFactory TableInfoFactory
        {
            get { return _tableInfoFactory; }
        }

        public MapperFactory MapperFactory
        {
            get { return _mapperFactory; }
        }

        public void TableForPoco<T>(Action<ITableInfo> cfg)
        {
            cfg.MustNotBeNull();
            var table = TableInfoFactory.GetInfo(typeof (T));
            cfg(table);
        }

        /// <summary>
        /// Registers mappings for a value object, in order to flatten it (insert/update helpers only) or to restore it.
        /// For simple conversion of a result to a value object use <see cref="SqlFuConfig.RegisterConverter"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="from">Flattens the value object</param>
        /// <param name="to">Restores the value object</param>
        public void MapValueObject<T>(Func<T, object> from, Func<object, T> to = null)
        {
            Converters.MapValueObject(from, to);
        }

        public void RegisterConverter<T>(Func<object, T> converter)
        {
            Converters.RegisterConverter(converter);
        }

        public void AddNamingConvention(Func<Type, bool> match, Func<Type, TableName> convention)
        {
            TableInfoFactory.AddNamingConvention(match, convention);
        }

        //#region Connection info

        //private string _connectionString;
        //public string ConnectionString
        //{
        //    get
        //    {
        //        if (_connectionString.IsNullOrEmpty())
        //        {
        //            _connectionString = GetConnectionString(ConnectionName);
        //        }
        //        return _connectionString;
        //    }
        //    set
        //    {
        //        _connectionString = value;
        //    }
        //}

        //public string GetConnectionString(string name)
        //{
        //    if (name.IsNullOrEmpty())
        //    {
        //        return ConfigurationManager.ConnectionStrings[0].ConnectionString;
        //    }
        //    return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        //}

        //public string ConnectionName { get; set; } 
        //#endregion

        #region Events

        private Action<DbCommand, Exception> _onException = (s, e) => { };

        public Action<DbCommand, Exception> OnException
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
        private string conexName;
        private string connString;


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



        private Dictionary<string, SqlProfile> _profiles = new Dictionary<string, SqlProfile>();

        public void AddProfile(IDbProvider provider, string connectionString, string name = "default")
        {
            provider.MustNotBeNull();
            _profiles[name] = new SqlProfile() {ConnectionString = connectionString, Name = name, Provider = provider};

        }

        public SqlProfile GetProfile(string name) => _profiles[name];

    }
}