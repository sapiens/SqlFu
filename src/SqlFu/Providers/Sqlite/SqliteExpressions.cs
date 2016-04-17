using System;

namespace SqlFu.Providers.Sqlite
{
    public class SqliteExpressions:DbProviderExpressions
    {
        public override string Day(string column) => $"strftime('%d',{column})";

        public override string Year(string column)
            => $"strftime('%Y',{column})";

        public override string Length(string column)
            => $"length({column})";

        public override string Substring(string column, int pos, int length)
        {
            column.MustNotBeNull();
            var idx = pos + 1;
            return $"substr({column},{idx},{length})";
        }
    }
}