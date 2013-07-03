using System.Collections.Generic;
using System.Data.Common;

namespace SqlFu
{
    internal class PreparePagedStatement : PrepareStatement
    {
        private readonly long _skip;
        private readonly int _take;

        public const string SkipParameterName = "_fuskip";
        public const string TakeParameterName = "_futake";

        public PreparePagedStatement(IHaveDbProvider provider, long skip, int take, string sql, params object[] args)
            : base(provider, sql, args)
        {
            _skip = skip;
            _take = take;
        }

        private string _select;

        public void SetupCount(DbCommand cmd)
        {
            SetupParameters(cmd);
            var count = "";
            Provider.MakePaged(_sql, out _select, out count);
            cmd.CommandText = count;
            Format(cmd);
        }

        public override void Setup(DbCommand cmd)
        {
            cmd.CommandText = _select;
            AddPagingParams(cmd);
            Format(cmd);
        }

        private void AddPagingParams(DbCommand cmd)
        {
            var sp = cmd.CreateParameter();
            Provider.SetupParameter(sp, SkipParameterName, _skip);
            cmd.Parameters.Add(sp);

            var tp = cmd.CreateParameter();
            Provider.SetupParameter(tp, TakeParameterName, _take);
            cmd.Parameters.Add(tp);
            var lc = new List<string>(ParamNames);
            lc.Add(SkipParameterName);
            lc.Add(TakeParameterName);
            ParamNames = lc.ToArray();
        }
    }
}