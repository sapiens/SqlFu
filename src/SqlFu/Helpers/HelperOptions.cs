using System;
using System.Data.Common;
using CavemanTools;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public class HelperOptions : IHelperOptions
    {
        public TableInfo Info { get; }
        private TableName _tableName;

        public TableName TableName
        {
            get { return _tableName; }
            set
            {
                value.MustNotBeNull();
                _tableName = value;
            }
        }

        public Action<DbCommand> CmdOptions { get; set; } = Empty.ActionOf<DbCommand>();

        public HelperOptions(TableInfo info)            
        {
            Info = info;
            _tableName = info.TableName;
        }
      
    }
}