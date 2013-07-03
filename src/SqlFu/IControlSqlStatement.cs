using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace SqlFu
{
    public interface IControlSqlStatement
    {
        int Execute();
        T GetValue<T>(Func<object, T> converter = null);
        IEnumerable<T> Query<T>(Func<IDataReader, T> mapper = null);
        T QuerySingle<T>(Func<IDataReader, T> mapper = null);
        IControlSqlStatement Apply(Action<DbCommand> action);
    }

    public class ControlledQueryStatement : IControlSqlStatement, IDisposable
    {
        private DbCommand _cmd;

        public ControlledQueryStatement(DbConnection cnx, string sql, params object[] args)
        {
            _cmd = cnx.CreateCommand();
            var statment = new PrepareStatement(cnx.GetProvider(), sql, args);
            statment.Setup(Command);
        }

        public DbCommand Command
        {
            get { return _cmd; }
        }

        public bool Reusable { get; set; }

        public int Execute()
        {
            var rez = Command.Execute();
            if (!Reusable) Dispose();
            return rez;
        }

        public T GetValue<T>(Func<object, T> converter = null)
        {
            var rez = Command.GetValue(converter);
            if (!Reusable) Dispose();
            return rez;
        }

        public IEnumerable<T> Query<T>(Func<IDataReader, T> mapper = null)
        {
            var rez = Command.Fetch(mapper);
            if (!Reusable) Dispose();
            return rez;
        }

        public T QuerySingle<T>(Func<IDataReader, T> mapper = null)
        {
            var rez = Command.Fetch(mapper, true);
            if (!Reusable) Dispose();
            return rez.FirstOrDefault();
        }

        public IControlSqlStatement Apply(Action<DbCommand> action)
        {
            action.MustNotBeNull();
            action(Command);
            return this;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_cmd != null)
            {
                _cmd.Dispose();
                _cmd = null;
            }
        }
    }
}