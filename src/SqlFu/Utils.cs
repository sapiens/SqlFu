using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlFu
{
    public static class Utils
    {
       
        public static string FormatCommand(this IDbCommand cmd)
        {
            return FormatCommand(cmd.CommandText, (cmd.Parameters.Cast<IDbDataParameter>().ToDictionary(p=>p.ParameterName,p=>p.Value)));
        }

        public static bool IsListParam(this object data)
        {
            if (data == null) return false;
            var value = data.GetType();
            return value.Implements<IEnumerable>() && typeof(string)!=value && typeof(byte[])!=value;
        }

        public static string FormatCommand(string sql, IDictionary<string,object> args)
        {
            var sb = new StringBuilder();
            if (sql == null)
                return "";
            sb.Append(sql);
            if (args != null && args.Count > 0)
            {
                sb.Append("\n");
                foreach(var kv in args)
                {
                    sb.AppendFormat("\t -> {0} [{1}] = \"{2}\"\n",  kv.Key, kv.Value.GetType().Name, kv.Value);
                }
                
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        public static bool IsCustomObjectType(this Type t)
        {
            return t.IsClass && (Type.GetTypeCode(t) == TypeCode.Object);
        }
    }
}