using System;
using System.Data;
using System.Data.Common;

namespace Tests
{
    class FakeTransaction : DbTransaction
    {
        #region Overrides of DbTransaction

        public override void Commit()
        {

        }

        public override void Rollback()
        {

        }

        protected override DbConnection DbConnection
        {
            get { throw new NotImplementedException(); }
        }

        public override IsolationLevel IsolationLevel
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}