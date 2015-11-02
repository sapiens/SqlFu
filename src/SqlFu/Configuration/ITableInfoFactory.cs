using System;
using System.Collections.Generic;
using SqlFu.Configuration.Internals;

namespace SqlFu.Configuration
{
    public interface ITableInfoFactory
    {
        TableInfo GetInfo(Type pocoType);
        //IEnumerable<ColumnInfo> GetColumnsInfo(Type pocoType);
    }
}