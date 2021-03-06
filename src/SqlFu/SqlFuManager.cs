using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using SqlFu.Configuration;
using SqlFu.Configuration.Internals;
using SqlFu.Executors;
using SqlFu.Mapping;
using SqlFu.Providers;


namespace SqlFu
{
    public static class SqlFuManager
    {
        private const string DefaultProfile = "default";
        static SqlFuConfig _config=new SqlFuConfig();

        internal static SqlFuConfig Config => _config;

        public static void ResetConfig()
        {
            _config=new SqlFuConfig();
        }

        public static void UseLogManager(this SqlFuConfig config)
        {
            
            config.OnCommand = cmd => "SqlFu".LogDebug(cmd.FormatCommand());
            config.OnException = (cmd, ex) =>
            {
                "SqlFu".LogDebug($"[SqlFu] Command threw exception {cmd.FormatCommand()}");
                "SqlFu".LogDebug(ex.ToString());
            };
        }

        /// <summary>
        /// Returns a <see cref="DbFactory"/> instance for the specified profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static IDbFactory GetDbFactory(string profile = DefaultProfile)
        {
            var accessProfile = Config.GetProfile(profile);
            if (accessProfile.Factory == null) accessProfile.Factory = new DbFactory(accessProfile,_config);
            return accessProfile.Factory;
        }

        /// <summary>
        /// Used to issue quick queries that are using <see cref="T"/> as a source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="profile"></param>
        /// <returns></returns>
      public static QueryOver<T> QueryOver<T>(string profile=DefaultProfile) where T : class =>new QueryOver<T>(GetDbFactory().ToConnectionFactory());

        /// <summary>
        /// Used to issue quick queries that are using <see cref="T"/> as a source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static  QueryOver<T> QueryOver<T>(this IDbFactory fac) where T : class =>new QueryOver<T>(fac.ToConnectionFactory());

        /// <summary>
        /// Used to issue quick queries that are using <see cref="T"/> as a source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static  QueryOver<T> QueryOver<T>(this IDbFactory fac,T anon,TableName name) where T : class =>new QueryOver<T>(fac.ToConnectionFactory(),name);
        
        
        /// <summary>
        /// Used to issue quick queries that are using <see cref="T"/> as a source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anonymous">Source to build query</param>
        /// <param name="name">Table/view name that the objects represents</param>
        /// <param name="profile"></param>
        public static QueryOver<T> QueryOver<T>(T anonymous, TableName name, string profile = DefaultProfile) where T : class
        {
            var f = GetDbFactory(profile).ToConnectionFactory();
           return new QueryOver<T>(f,name);
        }

        /// <summary>
        /// Returns a singleton factory instance implementing T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDbFactory<T>() where T : IDbFactory=>(T)Config.GetProfile<T>().Factory;
       
        /// <summary>
        /// Wraps an existing connection into a SqlFu connection. If provider is not specified, the default one is used
        /// </summary>
        /// <param name="conex"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(DbConnection conex, IDbProvider provider = null) => new SqlFuConnection(provider ?? Config.GetProfile().Provider, conex,Config);

        /// <summary>
        /// You can invoke this as many times as you want, only one configuration is created
        /// </summary>
        /// <param name="cfg"></param>
        public static void Configure(Action<SqlFuConfig> cfg)
        {
            cfg.MustNotBeNull();
            UseLogManager(Config);
            cfg(Config);
            if (_config.HasNoProfiles) throw new InvalidOperationException("You need to define a profile in order to continue");
        }

        public static DbConnection OpenConnection(IDbProvider provider,string connectionString)
        {
            provider.MustNotBeNull();
            if (connectionString == null)
            {
                  throw new InvalidOperationException(
                    "I need a connection! Either set SqlFuFactory.Config.Providers.ConnectionString method or define a connection in config file. If there are more than one connection defined, set SqlFuFactory.Config.Providers.ConnectionString");
            }
            var sql= new SqlFuConnection(provider,connectionString, Config);
            sql.Open();
            return sql;
        }
         public static async Task<DbConnection> OpenConnectionAsync(IDbProvider provider,string connectionString,CancellationToken cancel)
        {
            provider.MustNotBeNull();
            if (connectionString == null)
            {
                  throw new InvalidOperationException(
                    "I need a connection! Either set SqlFuFactory.Config.Providers.ConnectionString method or define a connection in config file. If there are more than one connection defined, set SqlFuFactory.Config.Providers.ConnectionString");
            }
            var sql= new SqlFuConnection(provider,connectionString, Config);
            await sql.OpenAsync(cancel).ConfigureAwait(false);
             return sql;
        }
        
        /// <summary>
        /// Creates a connection that is not open. To be used when you want to control when and how the connection should be open
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbConnection CreateConnection(IDbProvider provider,string connectionString)
        {
            provider.MustNotBeNull();
            if (connectionString == null)
            {
                  throw new InvalidOperationException(
                    "I need a connection! Either set SqlFuFactory.Config.Providers.ConnectionString method or define a connection in config file. If there are more than one connection defined, set SqlFuFactory.Config.Providers.ConnectionString");
            }
           return new SqlFuConnection(provider,connectionString, Config);          
        }


        /// <summary>
        /// Returns the table information that will be used for the poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static TableInfo GetPocoInfo<T>(this DbConnection db) => db.CastAs<SqlFuConnection>().Config.TableInfoFactory.GetInfo(typeof (T));


        public static IDbProvider Provider(this DbConnection cnx)
        {
            var sqlfu = cnx as SqlFuConnection;
            if (sqlfu != null)
            {
                return sqlfu.Provider;
            }
          
            throw new NotSupportedException("Only SqlFu connections are supported");
        }

        public static SqlFuConfig SqlConfig(this DbCommand cmd) => cmd.CastAs<SqlFuCommand>().SqlFuConnection.Config;

        public static Func<object, T> GetConverter<T>(this DbCommand cmd,Func<object, T> converter)
        {
            if (converter != null) return converter;

            return cmd.SqlConfig().Converters.Convert<T>;
        }

        /// <summary>
        /// Configure how the poco will be mapped to table
        /// </summary>
        /// <param name="cnx"></param>
        /// <param name="cfg"></param>
        /// <typeparam name="T"></typeparam>
        public static void ConfigureTableFor<T>(this DbConnection cnx, Action<ITableInfo<T>> cfg) where T : class
            => cnx.SqlFuConfig().ConfigureTableForPoco(cfg);
        
        /// <summary>
        /// Sets the table name for <see cref="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cnx"></param>
        /// <param name="name"></param>
        public static void TableNameFor<T>(this DbConnection cnx, TableName name) where T:class
        {
            cnx.SqlFuConfig().ConfigureTableForPoco<T>(s=>s.TableName=name);
        } 
        
        public static Func<DbDataReader, T> GetMapper<T>(this DbCommand cmd,Func<DbDataReader, T> mapper,string cmdText)
        {
            if (mapper != null) return mapper;
            return reader => cmd.SqlConfig().MapperFactory.Map<T>(reader, cmdText.GetCachingId());
        }

        public interface IRegisterTypeConverterForWriting<T>
        {
            IRegisterTypeConverterForReading<T> WriteAs(Func<T, object> write);
        }
        public interface IRegisterTypeConverterForReading<T>
        {
            IRegisterTypeConverterForWriting<T> ReadAs(Func<object, T> write);
        }

        public interface IRegisterTypeConverter<T>:IRegisterTypeConverterForWriting<T>,IRegisterTypeConverterForReading<T>
        {

        }

        class ConverterRegister<T> : IRegisterTypeConverter<T>
        {
            private readonly IRegisterConverter _cfg;

            public ConverterRegister(IRegisterConverter cfg)
            {
                _cfg = cfg;
            }
            public IRegisterTypeConverterForReading<T> WriteAs(Func<T, object> write)
            {
                _cfg.RegisterWriteConverter(write);
                return this;
            }

            public IRegisterTypeConverterForWriting<T> ReadAs(Func<object,T> read)
            {
                _cfg.RegisterConverter(read);
                return this;
            }
        }

        /// <summary>
        /// How SqlFu will handle value of type when writing/reading to/from db
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IRegisterTypeConverter<T> WhenType<T>(this SqlFuConfig cfg)=> new ConverterRegister<T>(Config.Converters);
    }
}