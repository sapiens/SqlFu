using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using CavemanTools.Logging;
using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers.SqlServer
{
    /// <summary>
    /// SQlServer 2012+
    /// </summary>
    public class SqlServer2012Provider:DbProvider
    {
        static Lazy<SqlServer2012Provider> _instance=new Lazy<SqlServer2012Provider>(()=>new SqlServer2012Provider());

        public static SqlServer2012Provider Instance => _instance.Value;
        

        public const string Id = "SqlServer2012";
        private const string ProviderName = "System.Data.SqlClient";

        public readonly SqlServerDbType DbTypes=new SqlServerDbType();
      
//        public readonly MsSqlFunctions Functions=new MsSqlFunctions();

        public SqlServer2012Provider():base(Id,ProviderName)
        {
            
        }

        public override string ParamPrefix
        {
            get { return "@"; }
        }

        public override string FormatIndexOptions(string idxDef, string options = "")
        {
            if (options.IsNullOrEmpty()) return idxDef;
            if (idxDef.Contains("key")) return FormatIdx(idxDef, options,"(");
            return FormatIdx(idxDef, options,"index");
        }

        private string FormatIdx(string idxDef, string options,string before)
        {
            var idx = idxDef.IndexOf(before);
            return idxDef.Substring(0, idx) + " " + options + " " + idxDef.Substring(idx);
        }


        public override string EscapeIdentifier(string name)
        {
            return Escape(name,"[","]");
        }

        public override string GetColumnType(Type type)
        {
            if (type.IsEnum()) type=type.GetUnderlyingTypeForEnum();
            return DbTypes.GetValueOrDefault(type);
        }

        public override string GetIdentityKeyword()
        {
          
              return "identity(1,1)";
           
        }

        public override bool IsDbBusy(DbException ex)
        {
            if (ex.Message.Contains("is not currently available"))
            {
                "SqlServer".LogWarn("Too many connections");
                return true;
            }

            if (ex.Message.Contains("imeout"))
            {
                "SqlServer".LogWarn("Connection timeout");
                return true;
            }
            return false;
        }

        public override bool IsUniqueViolation(DbException ex, string keyName = "")
        {
            if (!ex.Message.Contains("Cannot insert duplicate")) return false;
            return !keyName.IsNotEmpty() || ex.Message.Contains(keyName);
        }

      public override string GetSqlForDropTableIfExists(string name, string schema = null)
        {
            var table = this.EscapeTableName(name, schema);
          return $"IF OBJECT_ID('{table}', 'U') IS NOT NULL DROP TABLE {table}";
        }

        public override string AddReturnInsertValue(string values, string identityColumn)
        {
            if (identityColumn.IsNullOrEmpty()) return values;
            return $"\nOUTPUT INSERTED.{identityColumn} AS ID " + values;
        }

        protected override DbFunctions GetFunctions() => new SqlServerFunctions();
        

        public override void SetupParameter(IDbDataParameter param, string name, object value)
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
                if (value.Cast<DateTime>().Year < 1753)
                {
                    param.DbType=DbType.DateTime2;
                }
                return;
            }

            if (tp.IsValueType) return;

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

      
        public override IDbProviderExpressions GetExpressionsHelper()
        {
            return new SqlServer2012Expressions();
        }

        public override string FormatQueryPagination(string sql, Pagination page, ParametersManager pm)
        {
            if (!sql.Contains("order by")) sql += " order by 1";
            pm.AddValues(page.Skip, page.PageSize);
            return string.Format("{2} OFFSET @{0} ROWS FETCH NEXT @{1} ROWS ONLY",pm.CurrentIndex-2,pm.CurrentIndex-1 ,sql);
        }

        protected override IDatabaseTools GetTools()
        {
            return new SqlServerDbTools();
        }

     

      
    }
}