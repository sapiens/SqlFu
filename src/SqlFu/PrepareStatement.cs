using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

namespace SqlFu
{
    class PrepareStatement
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
        }

        protected void SetupParameters(DbCommand cmd)
        {
            ParamNames = new string[0];
            if (_args.Length > 0)
            {
                var paramDict = CreateParamsDictionary(_args);
                var allp = cmd.Parameters;
                List<string> pnames = new List<string>();
                var sb = new StringBuilder();
                var lastParamCount = _args.Length;

                IDbDataParameter p = null;

                foreach (var kv in paramDict)
                {
                    if (kv.Value.IsListParam())
                    {
                        var lp = kv.Value as IEnumerable;

                        sb.Clear();

                        foreach (var val in lp)
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

        internal static IDictionary<string, object> CreateParamsDictionary(params object[] args)
        {
            IDictionary<string, object> d = new Dictionary<string, object>();
            if (args != null)
            {
                if (args.Length == 1)
                {
                    var poco = args[0];
                    if (poco != null && !poco.IsListParam())
                    {
                        if (poco.GetType().IsCustomObjectType())
                        {
                            d = poco.ToDictionary();
                            return d;
                        }
                    }
                }

                for (int i = 0; i < args.Length; i++)
                {
                    d.Add(i.ToString(), args[i]);
                }
            }
            return d;
        }
    }
}