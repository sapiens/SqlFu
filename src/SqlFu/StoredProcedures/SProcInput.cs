using System;
using System.Data.Common;
using CavemanTools;

namespace SqlFu
{
    public class SProcInput
    {
        public string ProcName { get; set; }
        public object Arguments { get; set; }

        public Action<DbCommand> Apply { get; set; } = Empty.ActionOf<DbCommand>();
    }
}