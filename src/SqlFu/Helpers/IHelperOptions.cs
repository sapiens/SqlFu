using System;
using System.Data.Common;

namespace SqlFu
{
    public interface IHelperOptions
    {
        string TableName { get; set; }
        string DbSchema { get; set; }
        Action<DbCommand> CmdOptions { get; set; }
    }
}