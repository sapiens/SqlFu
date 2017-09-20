using System;
using System.Data.Common;
using CavemanTools;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public class HelperOptions : IHelperOptions
    {
        public TableName TableName { get; set; }
        public Action<DbCommand> CmdOptions { get; set; } = Empty.ActionOf<DbCommand>();

        public void EnsureTableName(TableInfo info)
        {
            if (TableName == null) TableName = info.TableName;            
        }

      
    }
}