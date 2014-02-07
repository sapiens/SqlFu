using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SqlFu
{
    internal class PrepareStatement
    {
        protected readonly IHaveDbProvider Provider;
        protected string _sql;
        private readonly object[] _args;
        protected string[] ParamNames;

        public PrepareStatement(IHaveDbProvider provider, string sql, params object[] args)
        {
            Provider = provider;
            _sql = sql;
            _args = args;
        }

        public virtual void Setup(DbCommand cmd)
        {
            SetupParameters(cmd);
            cmd.CommandText = _sql;
            Format(cmd);
        }

        protected void Format(DbCommand cmd)
        {
            cmd.CommandText = Provider.FormatSql(cmd.CommandText, ParamNames);
            Provider.OnCommandExecuting(cmd);
            if (SqlFuDao.EscapeMarkedIdentifiers)
            {
                cmd.CommandText = Provider.EscapeSql(cmd.CommandText);
            }            
        }

        protected void SetupParameters(DbCommand cmd)
        {
            ParamNames = new string[0];
            if (_args.Length > 0)
            {
                var paramDict = CreateParamsDictionary(_args);
                var allp = cmd.Parameters;
                List<string> pnames = new List<string>();
                StringBuilder sb =null;
                var lastParamCount = _args.Length;

                IDbDataParameter p = null;

                foreach (var kv in paramDict)
                {
                    if (kv.Value.IsListParam())
                    {
                        if (sb == null)
                        {
                            sb = new StringBuilder();
                        }
                        else sb.Clear();
                        
                        var listParam = kv.Value as IEnumerable;
                        
                        foreach (var val in listParam)
                        {
                            p = cmd.CreateParameter();

                            sb.Append("@" + lastParamCount + ",");
                            pnames.Add(lastParamCount.ToString());
                            Provider.SetupParameter(p, lastParamCount.ToString(), val);
                            allp.Add(p);
                            lastParamCount++;
                        }
                        sb.Remove(sb.Length - 1, 1);
                        _sql = _sql.Replace("@" + kv.Key, sb.ToString());
                    }
                    else
                    {
                        p = cmd.CreateParameter();
                        Provider.SetupParameter(p, kv.Key, kv.Value);
                        pnames.Add(kv.Key);
                        allp.Add(p);
                    }
                }

                ParamNames = pnames.ToArray();
            }
        }

    //internal static IDictionary<string, object> CreateParamsDictionary(params object[] args)

        private static CultureInfo culture = CultureInfo.InvariantCulture;

        internal static KeyValuePair<string, object>[] CreateParamsDictionary(params object[] args)
        {
           var d=new KeyValuePair<string, object>[args.Length];
            if (args != null)
            {
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
                    d[i]=new KeyValuePair<string, object>(i.ToString(culture),args[i]);                    
                }
            }
            return d;
        }
    }
}