using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Logging;
using SqlFu.Configuration.Internals;
using SqlFu.Executors;
using SqlFu.Providers;


namespace SqlFu
{
    public static class SqlFuManager
    {
        static SqlFuConfig config=new SqlFuConfig();

        public static SqlFuConfig Config => config;

        public static void ResetConfig()
        {
            config=new SqlFuConfig();
        }

        public static void UseLogManager()
        {
            Config.OnCommand = cmd => "SqlFu".LogDebug(cmd.FormatCommand());
            config.OnException = (cmd, ex) =>
            {
                "SqlFu".LogError("Command threw exception {0}", cmd.FormatCommand());
                "SqlFu".LogError(ex);
            };
        }

        /// <summary>
        /// Returns a <see cref="DbFactory"/> instance for the specified profile
        /// </summary>
        /// <param name="profile"></param>
        /// <returns></returns>
        public static IDbFactory GetDbFactory(string profile = "default")
        {
            var accessProfile = Config.GetProfile(profile);
            if (accessProfile.Factory == null) accessProfile.Factory = new DbFactory(accessProfile);
            return accessProfile.Factory;
        }

        /// <summary>
        /// Returns a singleton factory instance implementing T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetDbFactory<T>() where T : IDbFactory=>(T)Config.GetProfile<T>().Factory;
       
        [Obsolete("Use Config.AddProfile<IMyInterface>() to register the db interface and GetDbFactory<IMyInterface>(). This function will be removed in the next major version")]
        public static T GetDbFactory<T>(string name) where T:DbFactory,new()
        {
            var fact=new T();
            fact.Assign(config.GetProfile(name));
            return fact;
        }

        /// <summary>
        /// Wraps an existing connection into a SqlFu connection. If provider is not specified, the default one is used
        /// </summary>
        /// <param name="conex"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(DbConnection conex, IDbProvider provider = null) => new SqlFuConnection(conex, provider ?? Config.GetProfile().Provider);

        public static void Configure(Action<SqlFuConfig> cfg)
        {
            cfg.MustNotBeNull();
            UseLogManager();
            cfg(Config);
            if (config.HasNoProfiles) throw new InvalidOperationException("You need to define a profile in order to continue");
        }

        public static DbConnection OpenConnection(IDbProvider provider,string connectionString)
        {
            provider.MustNotBeNull();
            if (connectionString == null)
            {
                  throw new InvalidOperationException(
                    "I need a connection! Either set SqlFuFactory.Config.Providers.ConnectionString method or define a connection in config file. If there are more than one connection defined, set SqlFuFactory.Config.Providers.ConnectionString");
            }
            var sql= new SqlFuConnection(provider,connectionString);
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
            var sql= new SqlFuConnection(provider,connectionString);
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
           return new SqlFuConnection(provider,connectionString);          
        }


        /// <summary>
        /// Returns the table information that will be used for the poco
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <returns></returns>
        public static TableInfo GetPocoInfo<T>(this DbConnection db) => Config.TableInfoFactory.GetInfo(typeof (T));


        public static IDbProvider Provider(this DbConnection cnx)
        {
            var sqlfu = cnx as SqlFuConnection;
            if (sqlfu != null)
            {
                return sqlfu.Provider;
            }
          
            throw new NotSupportedException("Only SqlFu connections are supported");
        }
        

        public static Func<object, T> GetConverter<T>(Func<object, T> converter)
        {
            if (converter != null) return converter;
            return Config.Converters.Convert<T>;
        }

        public static Func<DbDataReader, T> GetMapper<T>(Func<DbDataReader, T> mapper,string cmdText)
        {
            if (mapper != null) return mapper;
            return reader => config.MapperFactory.Map<T>(reader, cmdText.GetCachingId());
        }
        
    }
}