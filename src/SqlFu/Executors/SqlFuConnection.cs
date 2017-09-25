using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Logging;
using SqlFu.Executors.Resilience;
using SqlFu.Providers;

namespace SqlFu.Executors
{

    public class SqlFuConnection : DbConnection
    {
        public SqlFuConfig Config { get; }
        private readonly Func<IRetryOnTransientErrorsStrategy> _errStrategyFactory;

        public SqlFuConnection(IDbProvider provider, string cnxString,SqlFuConfig config)
        {
            Config = config;
            _errStrategyFactory = Config.TransientErrorsStrategyFactory;
            Init(cnxString, provider);
        }

        public SqlFuConnection(IDbProvider provider, DbConnection cnx, SqlFuConfig config)
        {
            Config = config;
            cnx.MustNotBeNull();
            provider.MustNotBeNull();
            _conex = cnx;
            _provider = provider;
            _errStrategyFactory = config.TransientErrorsStrategyFactory;
        }

        
        private void Init(string cnxString, IDbProvider provider)
        {
            cnxString.MustNotBeEmpty();
            provider.MustNotBeNull();
            _provider = provider;
            _conex = _provider.CreateConnection();
            _conex.ConnectionString = cnxString;
         
        }

        public IRetryOnTransientErrorsStrategy CreateErrorStrategy() => _errStrategyFactory();
    
        private DbConnection _conex;

     


        public IDbProvider Provider => _provider;

        private IDbProvider _provider;

        #region DbConnection overrides

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return BeginSqlFuTransaction(isolationLevel);
        }

        /// <summary>
        /// Closes the connection to the database. This is the preferred method of closing any open connection.
        /// </summary>
        /// <exception cref="T:System.Data.Common.DbException">The connection-level error that occurred while opening the connection. </exception>
        public override void Close()
        {
            _conex.Close();
            SqlFuManager.Config.OnCloseConnection(this);
        }

        /// <summary>
        /// Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName">Specifies the name of the database for the connection to use.</param>
        public override void ChangeDatabase(string databaseName)
        {
            _conex.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString"/>.
        /// </summary>
        public override void Open()
        {
            var s = _errStrategyFactory();
      start:
            try
            {
                if (_conex.State != ConnectionState.Open) _conex.Open();
            }
            catch (DbException ex) when (Provider.IsTransientError(ex))
            {
                if (!s.CanRetry)
                {
                    throw;
                }
                var time=s.GetWaitingPeriod();
                Thread.Sleep(time);
                goto start;
            }

            SqlFuManager.Config.OnOpenConnection(this);
        }

        public override async Task OpenAsync(CancellationToken cancellationToken)
        {
            var s = _errStrategyFactory();
            start:
            try
            {
                if (_conex.State != ConnectionState.Open) await _conex.OpenAsync(cancellationToken).ConfigureFalse();
            }
            catch (DbException ex) when (Provider.IsTransientError(ex))
            {
                if (!s.CanRetry)
                {
                    throw;
                }
                var time = s.GetWaitingPeriod();
                await Task.Delay(time,cancellationToken).ConfigureFalse();
                goto start;
            }

            SqlFuManager.Config.OnOpenConnection(this);
        }

        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// </summary>
        /// <returns>
        /// The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection. The default value is an empty string.
        /// </returns>
        public override string ConnectionString { get; set; }

        /// <summary>
        /// Gets the name of the current database after a connection is opened, or the database name specified in the connection string before the connection is opened.
        /// </summary>
        /// <returns>
        /// The name of the current database or the name of the database to be used after a connection is opened. The default value is an empty string.
        /// </returns>
        public override string Database => _conex.Database;

        /// <summary>
        /// Gets a string that describes the state of the connection.
        /// </summary>
        /// <returns>
        /// The state of the connection. The format of the string returned depends on the specific type of connection you are using.
        /// </returns>
        public override ConnectionState State
        {
            get { return _conex.State; }
        }

        /// <summary>
        /// Gets the name of the database server to which to connect.
        /// </summary>
        /// <returns>
        /// The name of the database server to which to connect. The default value is an empty string.
        /// </returns>
        public override string DataSource
        {
            get { return _conex.DataSource; }
        }

        /// <summary>
        /// Gets a string that represents the version of the server to which the object is connected.
        /// </summary>
        /// <returns>
        /// The version of the database. The format of the string returned depends on the specific type of connection you are using.
        /// </returns>
        public override string ServerVersion
        {
            get { return _conex.ServerVersion; }
        }

        /// <summary>
        /// Creates and returns a <see cref="T:System.Data.Common.DbCommand"/> object associated with the current connection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbCommand"/> object.
        /// </returns>
        protected override DbCommand CreateDbCommand()
        {
            var cmd = new SqlFuCommand(this);
            if (_trans != null) cmd.Transaction = _trans;
          
            return cmd;
        }

        #endregion

        #region Transaction support

        private int _tLevel;


        public int TransactionDepth => _tLevel;

        /// <summary>
        /// Gets the underlying connection
        /// </summary>
        public DbConnection Connection => _conex;

        private DbTransaction _trans;

        public DbTransaction BeginSqlFuTransaction(IsolationLevel? isolationLevel = null)
        {
            _tLevel++;
            if (_tLevel == 1)
            {
                _trans = isolationLevel.HasValue
                             ? Connection.BeginTransaction(isolationLevel.Value)
                             : Connection.BeginTransaction();
            }
            return new MyTransactionWrapper(this);
        }

        internal void Commit()
        {
            EnsureTransaction();
            _tLevel--;
            if (_tLevel == 0)
            {
                _trans.Commit();
                _trans = null;
            }
        }

        internal void Rollback()
        {
            if (_trans != null)
            {
                _trans.Dispose();
                _trans = null;
                _tLevel = 0;
            }
        }

        private void EnsureTransaction()
        {
            if (_trans == null) throw new InvalidOperationException("No transaction started");
        }

        #region TrnsactionClass

        private class MyTransactionWrapper : DbTransaction
        {
            private SqlFuConnection _db;

            public MyTransactionWrapper(SqlFuConnection db)
            {
                _db = db;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_db != null)
                    {
                        Rollback();
                    }
                }
            }

            public override void Commit()
            {
                if (_db != null)
                {
                    _db.Commit();
                    _db = null;
                }
                else
                {
                    throw new InvalidOperationException("Transaction already finished");
                }
            }

            public override void Rollback()
            {
                _db.Rollback();
                _db = null;
            }

            protected override DbConnection DbConnection => _db.Connection;

            public override IsolationLevel IsolationLevel => _db._trans.IsolationLevel;
        }

        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (Connection == null) return;
            if (_trans != null)
            {
                Rollback();
            }
            Close();
            Connection.Dispose();           
            _conex = null;
        }
    }

    public class SqlFuCommand : DbCommand
    {
        private readonly SqlFuConnection _cnx;
        private DbCommand _cmd;
        
        public SqlFuCommand(SqlFuConnection cnx)
        {
            _cnx = cnx;
            
            _cmd = cnx.Connection.CreateCommand();
        }

        public IRetryOnTransientErrorsStrategy GetErrorsStrategy() => _cnx.CreateErrorStrategy();


        public static void HandleTransients(DbCommand cmd,Action sqlAction,IRetryOnTransientErrorsStrategy strat,IDbProvider provider)
        {
        
            start:
            try
            {
                SqlFuManager.Config.OnCommand(cmd);
                sqlAction();                
            }
            catch (DbException ex)
            {
                if (provider.IsTransientError(ex))
                {
                    "SqlFu".LogInfo("Transient error detected");
                    if (strat.CanRetry)
                    {
                        var period = strat.GetWaitingPeriod();
                        "SqlFu".LogInfo($"Waiting {period} before retrying");
                        Thread.Sleep(period);
                        "SqlFu".LogInfo("Retrying...");
                        goto start;
                    }
                    "SqlFu".LogWarn($"No more retries left. Tried {strat.RetriesCount} times. Throwing exception");
                }

                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task HandleTransientsAsync(DbCommand cmd,Func<CancellationToken,Task> sqlAction,IRetryOnTransientErrorsStrategy strat,IDbProvider provider,CancellationToken cancel)
        {
        start:
            try
            {
                SqlFuManager.Config.OnCommand(cmd);
                await sqlAction(cancel).ConfigureFalse();                
            }
            catch (DbException ex)
            {
                if (provider.IsTransientError(ex))
                {
                    "SqlFu".LogInfo("Transient error detected");
                    if (strat.CanRetry)
                    {
                        var period = strat.GetWaitingPeriod();
                        "SqlFu".LogInfo($"Waiting {period} before retrying");
                        await Task.Delay(period,cancel).ConfigureFalse();
                        "SqlFu".LogInfo("Retrying...");
                        goto start;
                    }
                    "SqlFu".LogWarn($"No more retries left. Tried {strat.RetriesCount} times. Throwing exception");
                }

                SqlFuManager.Config.OnException(cmd, ex);
                throw;
            }
        }
        void HandleTransients(Action sqlAction) => HandleTransients(_cmd, sqlAction, _cnx.CreateErrorStrategy(), _cnx.Provider);

        public override void Cancel()
            => _cmd.Cancel();

        public override int ExecuteNonQuery()
        {
            var rez = -1;
            HandleTransients(() => rez=_cmd.ExecuteNonQuery());
            return rez;
        }
        
        public override object ExecuteScalar()
        {
            object obj = DBNull.Value;
            HandleTransients(() => obj = _cmd.ExecuteScalar());
            return obj;
        }

        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken) => _cmd.ExecuteReaderAsync(behavior, cancellationToken);

        public override async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            var rez = -1;
            await HandleTransientsAsync(_cmd, async (t) => rez = await _cmd.ExecuteNonQueryAsync(t).ConfigureFalse(), GetErrorsStrategy(), _cnx.Provider, cancellationToken).ConfigureFalse();
            return rez;
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object obj = DBNull.Value;
            await HandleTransientsAsync(_cmd,async (t) => obj = await _cmd.ExecuteScalarAsync(t).ConfigureFalse(),GetErrorsStrategy(),_cnx.Provider,cancellationToken).ConfigureFalse();
            return obj;
        }

        public override void Prepare()
            => _cmd.Prepare();

        public override string CommandText
        {
            get { return _cmd.CommandText; }
            set { _cmd.CommandText = value; }
        }

        public override int CommandTimeout
        {
            get => _cmd.CommandTimeout;
            set => _cmd.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _cmd.CommandType;
            set => _cmd.CommandType = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _cmd.UpdatedRowSource;
            set => _cmd.UpdatedRowSource = value;
        }

        protected override DbConnection DbConnection
        {
            get => _cmd.Connection;
            set => _cmd.Connection = value;
        }

        protected override DbParameterCollection DbParameterCollection => _cmd.Parameters;
        protected override DbTransaction DbTransaction
        {
            get => _cmd.Transaction;
            set => _cmd.Transaction = value;
        }

        public override bool DesignTimeVisible
        {
            get => _cmd.DesignTimeVisible;
            set => _cmd.DesignTimeVisible = value;
        }

        protected override DbParameter CreateDbParameter()
            => _cmd.CreateParameter();

        public IDbProvider Provider => _cnx.Provider;

        public SqlFuConnection SqlFuConnection
        {
            get { return _cnx; }
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => _cmd.ExecuteReader(behavior);
    }
}