using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using SqlFu.DDL;

namespace SqlFu
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class SqlFuConnection : DbConnection
    {
        internal SqlFuConnection()
        {
            var cnx = ConfigurationManager.ConnectionStrings[0];
            if (cnx == null)
                throw new InvalidOperationException(
                    "I need a connection! Either call SqlFu.ConnectionStringIs() method or define a connection in config file. If there are more than one connection defined, call SqlFu.ConnectionNameIs() method");

            Init(cnx.ConnectionString, ProviderFactory.GetProviderByName(cnx.ProviderName));
        }

        public SqlFuConnection(string connectionStringName)
        {
            var cnx = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (cnx == null)
                throw new InvalidOperationException(
                    "Can't find connection '{0}' in the configuration file.".ToFormat(connectionStringName));

            Init(cnx.ConnectionString, ProviderFactory.GetProviderByName(cnx.ProviderName));
        }

        public SqlFuConnection(string cnxString, string provider)
        {
            Init(cnxString, ProviderFactory.GetProviderByName(provider));
        }

        public SqlFuConnection(string cnxString, DbEngine provider)
        {
            Init(cnxString, ProviderFactory.GetProvider(provider));
        }

        public SqlFuConnection(string cnxString, IHaveDbProvider provider)
        {
            Init(cnxString, provider);
        }

        public SqlFuConnection(DbConnection cnx,DbEngine engine)
        {
            cnx.MustNotBeNull();
            _conex = cnx;
            _provider = ProviderFactory.GetProvider(engine);
            _conex.Open();
        }

        private void Init(string cnxString, IHaveDbProvider provider)
        {
            cnxString.MustNotBeEmpty();
            provider.MustNotBeNull();
            _provider = provider;
            _conex = _provider.CreateConnection();
            _conex.ConnectionString = cnxString;
            Connection.Open();
        }


        public StoredProcedureResult ExecuteStoredProcedure(string sprocName, object arguments = null)
        {
            return Connection.ExecuteStoredProcedure(sprocName, arguments);
        }


        public IDatabaseTools DatabaseTools
        {
            get { return Provider.GetTools(this); }
        }

        private DbConnection _conex;

        //public DbConnection Connection
        //{
        //    get
        //    {
        //        if (_conex == null)
        //        {
        //            _conex = Provider.CreateConnection();
        //            _conex.ConnectionString = _cnxString;
        //            _conex.Open();
        //            SqlFu.OnOpenConnection(_conex);
        //        }
        //        return _conex;
        //    }
        //}


        public IHaveDbProvider Provider
        {
            get { return _provider; }
        }

        private IHaveDbProvider _provider;

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
            if (_conex.State != ConnectionState.Open) _conex.Open();
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
        public override string Database
        {
            get { return _conex.Database; }
        }

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
            var cmd = Connection.CreateCommand();
            if (_trans != null) cmd.Transaction = _trans;
            return cmd;
        }

        #endregion

        #region Transaction support

        private int _tLevel;


        public int TransactionDepth
        {
            get { return _tLevel; }
        }

        public DbConnection Connection
        {
            get { return _conex; }
        }

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

            protected override DbConnection DbConnection
            {
                get { return _db.Connection; }
            }

            public override IsolationLevel IsolationLevel
            {
                get { return _db._trans.IsolationLevel; }
            }
        }

        #endregion

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Connection != null)
                {
                    if (_trans != null)
                    {
                        Rollback();
                    }
                    Connection.Close();
                    Connection.Dispose();
                    _conex = null;
                }
            }
        }
    }
}