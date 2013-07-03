using System;
using System.Data.Common;
using CavemanTools;

namespace SqlFu
{
    public sealed class DatabaseUnitOfWork : IUnitOfWork
    {
        private DbTransaction _trans;

        public DatabaseUnitOfWork(DbTransaction trans)
        {
            trans.MustNotBeNull();
            _trans = trans;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_trans != null)
            {
                _trans.Dispose();
                _trans = null;
            }
        }

        public void Commit()
        {
            _trans.Commit();
        }

        public string Tag { get; set; }
    }
}