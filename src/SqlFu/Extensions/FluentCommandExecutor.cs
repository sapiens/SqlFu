using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders;

namespace SqlFu
{
    class FluentCommandExecutor<T>:IProcessEachRow<T>
    {
        private readonly bool _disposeAfterQuery;
        private bool _disposed = false;
        private readonly CancellationToken _cancel;
        private DbCommand _cmd;

        public FluentCommandExecutor(DbConnection cnx, IGenerateSql sqlGen, Action<DbCommand> cfg,bool disposeAfterQuery=false, CancellationToken? cancel=null)
        {
            _disposeAfterQuery = disposeAfterQuery;                                                             
            _cancel = cancel??CancellationToken.None;

            var cmdConfig = sqlGen.GetCommandConfiguration();
            cmdConfig.ApplyOptions = cfg;
            _cmd = cnx.CreateAndSetupCommand(cmdConfig);
        }

        void Dispose()
        {
            if(!_disposeAfterQuery || _disposed) return;
            _cmd.Connection.Dispose();
            _cmd.Dispose();
            _disposed = true;
        }

        private void CheckDispose()
        {
            if (_disposeAfterQuery && _disposed)
                throw new ObjectDisposedException("FluentCommandExecutor",
                    "Query executor has already disposed the connection");
        }

        public async Task<T> GetValueAsync()
        {
            CheckDispose();
            var r= await _cmd.GetValueAsync<T>(_cancel).ConfigureFalse();
            Dispose();
            return r;
        }

        public async Task<T> GetFirstRowAsync()
        {
            CheckDispose();
            T result = default(T);
            await _cmd.QueryAndProcessAsync<T>(_cancel,r =>
            {
                result = r;
                return true;
            }, firstRowOnly: true).ConfigureFalse();
            Dispose();
            return result;
        }

        public T GetValue()
        {
            CheckDispose();
            var r = _cmd.GetValue<T>();
            Dispose();
            return r;
        }

        public T GetFirstRow()
        {
            CheckDispose();
            T result = default(T);
            _cmd.QueryAndProcess<T>(r =>
            {
                result = r;
                return true;
            },firstRowOnly:true);
            Dispose();
            return result;
        }


        public List<T> GetRows()
        {
            CheckDispose();
            var r = _cmd.Fetch<T>();
            Dispose();
            return r;
        }

        public async Task<List<T>> GetRowsAsync()
        {
            CheckDispose();
            var r = await _cmd.FetchAsync<T>(_cancel).ConfigureFalse();
            Dispose();
            return r;
        }

        public IQueryAndProcess ProcessEachRow(Func<T, bool> processor)
        {
            CheckDispose();
            processor.MustNotBeNull();
            return new QueryAndProcess(_cmd,processor,_cancel,_disposeAfterQuery);
        }

        class QueryAndProcess:IQueryAndProcess
        {
            private readonly DbCommand _cmd;
            private readonly Func<T, bool> _processor;
            private readonly CancellationToken _cancel;
            private readonly bool _disposeAfterQuery;

            public QueryAndProcess(DbCommand cmd, Func<T, bool> processor, CancellationToken cancel,
                bool disposeAfterQuery)
            {
                _cmd = cmd;
                _processor = processor;
                _cancel = cancel;
                _disposeAfterQuery = disposeAfterQuery;
            }

            public void Execute() => _cmd.QueryAndProcess(_processor,disposeConnection:_disposeAfterQuery);

            public Task ExecuteAsync()
                => _cmd.QueryAndProcessAsync(_cancel, _processor,disposeConnection:_disposeAfterQuery);
        }
    }
}