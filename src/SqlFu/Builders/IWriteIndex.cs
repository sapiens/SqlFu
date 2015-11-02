using System.Collections.Generic;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders
{
    public interface IWriteIndex
    {
        IWriteIndex Create();
        IWriteIndex Index(string name);
        IWriteIndex Unique();
        IWriteIndex OnColumns(IEnumerable<ColumnInfo> columns);
    }
}