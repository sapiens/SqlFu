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
    public class SqlFuCommand : DbCommand
    {
        private readonly SqlFuConnection _cnx;
        private DbCommand _cmd;
        private SqlFuConfig _cfg;

        public SqlFuCommand(SqlFuConnection cnx)
        {
            _cnx = cnx;
            _cfg = cnx.SqlFuConfig();
            _cmd = cnx.Connection.CreateCommand();
        }

        public IRetryOnTransientErrorsStrategy GetErrorsStrategy() => _cnx.CreateErrorStrategy();


        public static void HandleTransients(DbCommand cmd,Action sqlAction,IRetryOnTransientErrorsStrategy strat,IDbProvider provider,SqlFuConfig cfg)
        {
          //  var cfg = cmd.SqlConfig();
            start:
            try
            {
                cfg.OnCommand(cmd);
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

                cfg.OnException(cmd, ex);
                throw;
            }
        }

        public static async Task HandleTransientsAsync(DbCommand cmd,Func<CancellationToken,Task> sqlAction,IRetryOnTransientErrorsStrategy strat,IDbProvider provider,CancellationToken cancel,SqlFuConfig config)
        {
         start:
            try
            {
                config.OnCommand(cmd);
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

                config.OnException(cmd, ex);
                throw;
            }
        }
        void HandleTransients(Action sqlAction) => HandleTransients(_cmd, sqlAction, _cnx.CreateErrorStrategy(), _cnx.Provider,_cfg);

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
            await HandleTransientsAsync(_cmd, async (t) => rez = await _cmd.ExecuteNonQueryAsync(t).ConfigureFalse(), GetErrorsStrategy(), _cnx.Provider, cancellationToken,_cfg).ConfigureFalse();
            return rez;
        }

        public override async Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            object obj = DBNull.Value;
            await HandleTransientsAsync(_cmd,async (t) => obj = await _cmd.ExecuteScalarAsync(t).ConfigureFalse(),GetErrorsStrategy(),_cnx.Provider,cancellationToken,_cfg).ConfigureFalse();
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