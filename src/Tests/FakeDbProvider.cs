using System;
using System.Collections.Generic;
using System.Data.Common;
using CavemanTools;
using CavemanTools.Model;
using SqlFu;
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
            Converters = Setup.Converters();
        }
        public FakeDbProvider(Func<DbConnection> factory, string providerId) : base(factory, providerId)
        {
            
        }

        protected override EscapeIdentifierChars GetEscapeIdentifierChars()
        => new EscapeIdentifierChars();

        public override string ParamPrefix { get; } = "@";

        

       

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

        public override string CreateInsertSql(InsertSqlOptions options, IDictionary<string, object> columnValues)
          =>  $"insert into {EscapeTableName(options.TableName)} ({columnValues.Keys.StringJoin()})\n values({JoinValuesAsParameters(columnValues)})";
     
        public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm)
            => sql + " limit";

      
      
    }
}