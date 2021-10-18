using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers
{
    public class GenericProvider : DbProvider
    {
        private readonly EscapeIdentifierChars _escapeChars;
        private const string Id = "Generic";
        public GenericProvider(Func<DbConnection> factory, EscapeIdentifierChars escapeChars, SqlFuConfig config = null) : base(factory, Id, config)
        {
            _escapeChars = escapeChars;
        }

        protected override EscapeIdentifierChars GetEscapeIdentifierChars()
            => _escapeChars;

        public override string ParamPrefix { get; }

        protected override bool IsDbBusy(DbException ex)
            => false;

        public override bool IsUniqueViolation(DbException ex, string keyName = "")
        {
            return ex.Message.Contains("violation");
        }

        //public override bool ObjectExists(DbException ex, string name = null)
        //{
        //    return ex.Message.Contains("already exists");
        //}

        public override string CreateInsertSql(InsertSqlOptions options, IDictionary<string, object> columnValues)
        {
            return $"insert into {EscapeTableName(options.TableName)}({columnValues.Keys.Select(EscapeIdentifier).StringJoin()})\n values({JoinValuesAsParameters(columnValues)})";
        }

        public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm)
        {
            throw new NotSupportedException();
        }
    }
}