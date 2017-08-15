using System;

namespace SqlFu.Builders
{
    [Obsolete("All db/table create/drop related code will be removed next major version")]
    public enum TableExistsAction
    {
        Throw,
        DropIt,
        Ignore
    }
}