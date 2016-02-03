using System;
using System.Data.Common;
using CavemanTools;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public class HelperOptions : IHelperOptions
    {
        public string TableName { get; set; }
        public string DbSchema { get; set; }
        public Action<DbCommand> CmdOptions { get; set; } = Empty.ActionOf<DbCommand>();

        public void EnsureTableName(TableInfo info)
        {
            if (TableName.IsNullOrEmpty()) TableName = info.Table.Name;
            if (DbSchema.IsNullOrEmpty()) DbSchema = info.Table.Schema;
        }

        public  TableName Table=>new TableName(TableName,DbSchema);
    }
}