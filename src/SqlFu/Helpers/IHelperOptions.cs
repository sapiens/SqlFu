using System;
using System.Data.Common;
using SqlFu.Configuration;

namespace SqlFu
{
    public interface IHelperOptions
    {
        TableName TableName { get; set; }
        Action<DbCommand> CmdOptions { get; set; }
    }
}