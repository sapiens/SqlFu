using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
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
       
        public const string Id = "SqlServer2012";
      
        public readonly SqlServerType DbTypes=new SqlServerType();
      

        public SqlServer2012Provider(Func<DbConnection> factory):base(factory,Id)
        {
           
        }

        protected override EscapeIdentifierChars GetEscapeIdentifierChars()
      => new EscapeIdentifierChars('[', ']');

        public override string ParamPrefix => "@";

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



        public override string GetColumnType(Type type)
        {
            if (type.IsEnumType()) type=type.GetUnderlyingTypeForEnum();
            return DbTypes.GetValueOrDefault(type);
        }

        public override string GetIdentityKeyword() => "identity(1,1)";

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

        public override bool ObjectExists(DbException ex, string name = null)
            => ex.Message.Contains("already an object named") && (ex.Message.Contains(name ?? " "));



        public override string AddReturnInsertValue(string sqlValues, string identityColumn)
        {
            if (identityColumn.IsNullOrEmpty()) return sqlValues;
            return $"\nOUTPUT INSERTED.{identityColumn} AS ID " + sqlValues;
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

        protected override IDatabaseTools InitTools() => new SqlServerDbTools(this);
        protected override IDbProviderExpressions InitExpressionHelper()
        =>new DbProviderExpressions();
    }
}