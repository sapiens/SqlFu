using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers.Sqlite
{
    public class SqliteProvider:DbProvider
    {
        public const string Id = "Sqlite";       


        public SqliteProvider(Func<DbConnection> factory) : base(factory, Id)
        {
            InitExpressionHelper=()=>new SqliteExpressions();
            DbSupportsSchemas = false;
        }

        protected override EscapeIdentifierChars GetEscapeIdentifierChars()
        =>new EscapeIdentifierChars('[',']');

        public override string ParamPrefix { get; } = "@";

        protected override bool IsDbBusy(DbException ex)
        {
            return ex.Message.Contains("locked");
        }

        public override bool IsUniqueViolation(DbException ex, string keyName = "")
        {
            if (!ex.Message.Contains("UNIQUE constraint failed:")) return false;            
            if (keyName.IsNullOrEmpty()) return true;
            return ex.Message.Contains(keyName);
        }

        //public override bool ObjectExists(DbException ex, string name = null)
        //{
        //    if (!ex.Message.Contains("already exists")) return false;
        //    if (name.IsNullOrEmpty()) return true;
        //    return ex.Message.Contains(name);
        //}

        public override string CreateInsertSql(InsertSqlOptions options, IDictionary<string, object> columnValues)
        {
          return $"insert into {EscapeTableName(options.TableName)} ({columnValues.Keys.Select(EscapeIdentifier).StringJoin()})" +
                   $"\n values({JoinValuesAsParameters(columnValues)});SELECT last_insert_rowid()";
        }

       public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm) 
            => $"{sql} limit {page.Skip},{page.PageSize}";

        
    }
}