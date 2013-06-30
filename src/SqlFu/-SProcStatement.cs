using System;
using System.Collections.Generic;
using System.Data;

namespace SqlFu
{
    internal class _SProcStatement : SqlStatement
    {
        private readonly List<IDbDataParameter> _output = new List<IDbDataParameter>();
        private IDbDataParameter _return;

        public _SProcStatement(SqlFuConnection db) : base(db)
        {
        }

        public void UseStoredProcedure(string name, object args)
        {
            name.MustNotBeEmpty();
            _cmd.CommandType = CommandType.StoredProcedure;
            _cmd.CommandText = name;

            var provider = _db.Provider;
            var paramDict = CreateParamsDictionary(args);
            var allp = _cmd.Parameters;

            IDbDataParameter p = null;

            p = _cmd.CreateParameter();
            provider.SetupParameter(p, "ReturnValue", 0);
            allp.Add(p);
            _return = p;
            p.Direction = ParameterDirection.ReturnValue;

            foreach (var kv in paramDict)
            {
                p = _cmd.CreateParameter();
                var pname = kv.Key;
                if (kv.Key.StartsWith("_"))
                {
                    pname = kv.Key.Substring(1);
                    p.Direction = ParameterDirection.Output;
                }
                provider.SetupParameter(p, pname, kv.Value);
                allp.Add(p);
                _output.Add(p);
            }
        }

        protected override IDictionary<string, object> CreateParamsDictionary(params object[] args)
        {
            if (args.Length == 1 && args[0] == null) return base.CreateParamsDictionary();
            return base.CreateParamsDictionary(args);
        }

        public new StoredProcedureResult Execute()
        {
            base.Execute();
            var rez = new StoredProcedureResult();
            rez.ReturnValue = _return.Value.ConvertTo<int>();
            foreach (var p in _output)
            {
                //first char of parameter name is db specific param naming
                rez.OutputValues[p.ParameterName.Substring(1)] = p.Value;
            }

            return rez;
        }
    }
}