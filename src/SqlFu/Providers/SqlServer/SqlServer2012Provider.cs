using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers.SqlServer
{
    /// <summary>
    /// SQlServer 2012+
    /// </summary>
    public class SqlServer2012Provider:DbProvider
    {
       
        public const string Id = "SqlServer2012";             
      

        public SqlServer2012Provider(Func<DbConnection> factory):base(factory,Id)
        {
           InitExpressionHelper=()=>new SqlServer2012Expressions();
        }

        protected override EscapeIdentifierChars GetEscapeIdentifierChars()
      => new EscapeIdentifierChars('[', ']');

        public override string ParamPrefix => "@";

        private static string[] _transientErrors = new[]
        {
            "40197","40501","10053","10054","10060","40613","40143","233","64"
        };

        public override bool IsDbBusy(DbException ex)
        {
            if (_transientErrors.Any(err => ex.Message.Contains(err)))
            {
                "SqlServer".LogDebug(ex.Message);
                return true;
            }

          if (ex.Message.Contains("imeout"))
            {
                "SqlServer".LogDebug("Connection timeout");
                return true;
            }
            return false;
        }

        public override bool IsUniqueViolation(DbException ex, string keyName = "")
        {
            if (!ex.Message.Contains("Cannot insert duplicate")) return false;
            if (!keyName.IsNotEmpty()) return true;
            return ex.Message.Contains(keyName);
        }

        public override bool ObjectExists(DbException ex, string name = null)
            => ex.Message.Contains("already an object named") && (ex.Message.Contains(name ?? " "));



      public override string CreateInsertSql(InsertSqlOptions options, IDictionary<string, object> columnValues)
        {
            var ins = options.IdentityColumn.IsNullOrEmpty() ? "" : $"\nOUTPUT INSERTED.{options.IdentityColumn} AS ID ";
            return $"insert into {EscapeTableName(options.TableName)}({columnValues.Keys.Select(EscapeIdentifier).StringJoin()})" +ins+
                   $"\n values({JoinValuesAsParameters(columnValues)})";
        }


        public override void SetupParameter(DbParameter param, string name, object value)
        {
            base.SetupParameter(param, name, value);
            
            if (value == null) return;

            var tp = value.GetType();

           
            if (tp==typeof(string))
            {
                param.Size = Math.Max(((string)value).Length + 1, 4000);
                return;
            }

            if (tp == typeof (DateTime))
            {
                //var dt = (DateTime) value;
                //if (dt.Year < 1753)
                //{
                    param.DbType=DbType.DateTime2;
              //  }
                return;
            }

            if (tp.IsValueType()) return;

            if (tp.Name == "SqlGeography") //SqlGeography is a CLR Type
            {
                dynamic p = param;
                p.UdtTypeName = "geography";
                return;
            }

            if (tp.Name == "SqlGeometry") //SqlGeometry is a CLR Type
            {
                dynamic p = param;
                p.UdtTypeName = "geometry";
            }
        }

      
      

        public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm)
        {
            if (!sql.Contains("order by")) sql += " order by 1";
            pm.AddValues(page.Skip, page.PageSize);
            return string.Format("{2} OFFSET @{0} ROWS FETCH NEXT @{1} ROWS ONLY",pm.CurrentIndex-2,pm.CurrentIndex-1 ,sql);
        }
    }
}