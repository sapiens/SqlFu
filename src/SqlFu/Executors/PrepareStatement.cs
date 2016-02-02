using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using SqlFu.Providers;

namespace SqlFu.Executors
{
    public static class PrepareStatement
    {

        public static DbParameter AddParam(this DbCommand cmd, IDbProvider provider, string name, object value)
        {
            var p = cmd.CreateParameter();
            provider.SetupParameter(p,name,value);
            cmd.Parameters.Add(p);
            return p;
        }
       
       public  static string[] SetupParameters(this DbCommand cmd,IDbProvider provider,object[] args)
       {
           if (args.Length == 0) return new string[0];

            var paramDict = CreateParamsDictionary(args);
            var allp = cmd.Parameters;
            var pnames = new List<string>();
            StringBuilder sb = new StringBuilder();
            var lastParamCount = args.Length;
          
            DbParameter p = null;

            foreach (var kv in paramDict)
            {
                if (kv.Value.IsListParam())
                {
                    sb.Clear();

                    var listParam = kv.Value as IEnumerable;
                    //create and add parameters from list argument
                    foreach (var val in listParam)
                    {
                        p = cmd.CreateParameter();

                        sb.Append("@" + lastParamCount + ",");
                        pnames.Add(lastParamCount.ToString());
                        provider.SetupParameter(p, lastParamCount.ToString(),val);
                        allp.Add(p);
                        lastParamCount++;
                    }
                    sb.Remove(sb.Length - 1, 1);
                    cmd.CommandText = cmd.CommandText.Replace("@" + kv.Key, sb.ToString());                    
                }
                else
                {
                    p = cmd.CreateParameter();
                    provider.SetupParameter(p, kv.Key, kv.Value);
                    pnames.Add(kv.Key);
                    allp.Add(p);
                }
            }
            
            return pnames.ToArray();
        }


        private static CultureInfo culture = CultureInfo.InvariantCulture;

        public static KeyValuePair<string, object>[] CreateParamsDictionary(params object[] args)
        {
            var d = new KeyValuePair<string, object>[args.Length];
            if (args.Length == 1)
            {
                var poco = args[0];
                if (poco != null)
                {
                    if (!poco.IsListParam() && poco.IsCustomObject())
                    {
                        return poco.ToDictionary().ToArray();
                    }
                }

            }

            for (int i = 0; i < args.Length; i++)
            {
                d[i] = new KeyValuePair<string, object>(i.ToString(culture), args[i]);
            }
            return d;
        }
    }
}