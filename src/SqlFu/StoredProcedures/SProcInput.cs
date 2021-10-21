using System;
using System.Data.Common;
using CavemanTools;

namespace SqlFu
{
    public class SProcInput
    {
        public string ProcName { get; set; }
        /// <summary>
        /// Arguments as an anonymous object, output parameters names must be prefixed with _ 
        /// </summary>
        /// <example>
        /// new{Id=1,_OutValue=""}
        /// </example>
        public object Arguments { get; set; }

        /// <summary>
        /// Applied just before executing the command
        /// </summary>
        public Action<DbCommand> Apply { get; set; } = Empty.ActionOf<DbCommand>();
    }
}