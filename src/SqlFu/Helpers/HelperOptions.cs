using System;
using System.Data.Common;
using CavemanTools;
using SqlFu.Configuration;

namespace SqlFu
{
    public class HelperOptions : IHelperOptions
    {
        public string TableName { get; set; }
        public string DbSchema { get; set; }
        public Action<DbCommand> CmdOptions { get; set; } = Empty.ActionOf<DbCommand>(); 
        
        public  TableName Table=>new TableName(TableName,DbSchema);
    }
}