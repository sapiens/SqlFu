using System;
using System.Data.Common;
using System.Runtime.InteropServices;
using CavemanTools;
using CavemanTools.Model;
using SqlFu.Builders;
using SqlFu.Providers;
using Tests._Fakes;

namespace Tests
{
    public class FakeDbProvider:DbProvider
    {
        private FakeEscapeIdentifier _e;

        public FakeDbProvider():base(Empty.Func<DbConnection>(),"")
        {
            _e=new FakeEscapeIdentifier();
        }
        public FakeDbProvider(Func<DbConnection> factory, string providerId) : base(factory, providerId)
        {

        }

        public override string ParamPrefix { get; } = "@";

        public override string EscapeIdentifier(string name)
            => _e.EscapeIdentifier(name);

        public override string GetColumnType(Type type)
        {
            throw new NotImplementedException();
        }

        public override string GetIdentityKeyword()
        {
            throw new NotImplementedException();
        }

        public override bool IsDbBusy(DbException ex)
        {
            throw new NotImplementedException();
        }

        public override bool IsUniqueViolation(DbException ex, string keyName = "")
        {
            throw new NotImplementedException();
        }

        public override bool ObjectExists(DbException ex, string name = null)
        {
            throw new NotImplementedException();
        }

        public override string AddReturnInsertValue(string sqlValues, string identityColumn)
            => sqlValues;

        public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm)
            => sql + " limit";

        protected override IDatabaseTools InitTools()
        {
            throw new NotImplementedException();
        }

        protected override IDbProviderExpressions InitExpressionHelper()
        {
            throw new NotImplementedException();
        }
    }
}