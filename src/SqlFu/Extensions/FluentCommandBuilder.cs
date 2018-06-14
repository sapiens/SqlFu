using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using SqlFu.Builders;

namespace SqlFu
{
    class FluentCommandBuilder<T>:IProcessEachRow<T>
    {

        private readonly CancellationToken _cancel;
        private DbCommand _cmd;

        public FluentCommandBuilder(DbConnection cnx, IGenerateSql sqlGen, Action<DbCommand> cfg, CancellationToken? cancel)
        {
      
            _cancel = cancel??CancellationToken.None;

            var cmdConfig = sqlGen.GetCommandConfiguration();
            cmdConfig.ApplyOptions = cfg;
            _cmd = cnx.CreateAndSetupCommand(cmdConfig);
        }

        public Task<T> GetValueAsync()
            => _cmd.GetValueAsync<T>(_cancel);

        public async Task<T> GetFirstRowAsync()
        {
            T result = default(T);
            await _cmd.QueryAndProcessAsync<T>(_cancel,r =>
            {
                result = r;
                return true;
            }, firstRowOnly: true).ConfigureFalse();
            return result;
        }

        public T GetValue() => _cmd.GetValue<T>();

        public T GetFirstRow()
        {
            T result = default(T);
            _cmd.QueryAndProcess<T>(r =>
            {
                result = r;
                return true;
            },firstRowOnly:true);
            return result;
        }


        public List<T> GetRows()
            => _cmd.Fetch<T>();

        public Task<List<T>> GetRowsAsync()
            => _cmd.FetchAsync<T>(_cancel);

        public IQueryAndProcess ProcessEachRow(Func<T, bool> processor)
        {
            processor.MustNotBeNull();
            return new QueryAndProcess(_cmd,processor,_cancel);
        }

        class QueryAndProcess:IQueryAndProcess
        {
            private readonly DbCommand _cmd;
            private readonly Func<T, bool> _processor;
            private readonly CancellationToken _cancel;

            public QueryAndProcess(DbCommand cmd, Func<T, bool> processor, CancellationToken cancel)
            {
                _cmd = cmd;
                _processor = processor;
                _cancel = cancel;
            }

            public void Execute() => _cmd.QueryAndProcess(_processor);

            public Task ExecuteAsync()
                => _cmd.QueryAndProcessAsync(_cancel, _processor);
        }
    }
}