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

        public static SqlFuConfig Config
        {
            get { return config; }
        }

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

        public static IDbFactory GetDbFactory(string profile = "default") =>new DbFactory(Config.GetProfile(profile));

        public static T GetDbFactory<T>(string name) where T:DbFactory,new()
        {
            var fact=new T();
            fact.Assign(config.GetProfile(name));
            return fact;
        }
            


        ///// <summary>
        ///// Creates a connection using a registered provider. Use it when working with multiple databases.
        ///// A provider must be registered with SqlFuFactory.Config.Providers.Add().
        ///// Don't use it if you only have a single connection and a default provider.
        ///// </summary>     
        ///// <returns></returns>
        //public static DbConnection OpenConnection(string providerName)
        //{
        //  providerName.MustNotBeEmpty();
        //    var t = Config.Providers.Items.GetValueOrDefault(providerName);
        //    if (t == null)
        //    {
        //        throw new InvalidOperationException("There's no provider '{0}' registered".ToFormat(providerName));
        //    }
        //    return OpenConnection(t.Item1, t.Item2);
        //}

        /// <summary>
        /// Wraps an existing connection into a SqlFu connection. If provider is not specified, the default one is used
        /// </summary>
        /// <param name="conex"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DbConnection GetConnection(DbConnection conex, IDbProvider provider = null) => new SqlFuConnection(conex, provider ?? DefaultProvider);

        public static void Configure(Action<SqlFuConfig> cfg)
        {
            cfg.MustNotBeNull();
            UseLogManager();
            cfg(Config);
        }

        //todo set a null provider
        public static IDbProvider DefaultProvider { get; set; }

       public static DbConnection OpenConnection(IDbProvider provider,string connectionString)
        {
            provider.MustNotBeNull();
            if (connectionString == null)
            {
                  throw new InvalidOperationException(
                    "I need a connection! Either set SqlFuFactory.Config.Providers.ConnectionString method or define a connection in config file. If there are more than one connection defined, set SqlFuFactory.Config.Providers.ConnectionString");
            }
            var sql= new SqlFuConnection(provider,connectionString);
            sql.Connection.Open();
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
            await sql.Connection.OpenAsync(cancel).ConfigureAwait(false);
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



        public static TableInfo GetPocoInfo<T>(this DbConnection db)
        {
            return Config.TableInfoFactory.GetInfo(typeof (T));
        }
            

        public static IDbProvider GetProvider(this DbConnection cnx)
        {
            var sqlfu = cnx as SqlFuConnection;
            if (sqlfu != null)
            {
                return sqlfu.Provider;
            }
            //var type = cnx.GetType().Name;

            //if (type.Equals("SqlConnection", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return new SqlServerProvider();
            //}

            //if (type.StartsWith("MySql"))
            //{
            //    return new MySqlProvider();
            //}

            //if (type.StartsWith("Npgsql"))
            //{
            //    return new PostgresProvider();
            //}

            //if (type.StartsWith("SQLite"))
            //{
            //    return new SqliteProvider();
            //}

            //if (type.Equals("SqlCeConnection", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    return new SqlServerCEProvider();
            //}

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
            return reader => config.MapperFactory.Map<T>(reader, cmdText.GetUniqueHash());
        }
        
    }
}