using System;
using System.Data.Common;
using CavemanTools;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu
{
    public class HelperOptions : IHelperOptions
    {
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

        //public void EnsureTableName(TableInfo info)
        //{
        //    if (TableName == null) TableName = info.TableName;            
        //}

      
    }
}