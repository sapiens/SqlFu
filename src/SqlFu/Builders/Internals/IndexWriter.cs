using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.Internals
{
    class IndexWriter : IWriteIndex
    {
        private readonly StringBuilder _sb;
        private readonly IDbProvider _provider;

        public IndexWriter(StringBuilder sb,IDbProvider provider)
        {
            _sb = sb;
            _provider = provider;
        }

        public IWriteIndex Create()
        {
            _sb.Append("create");
            return this;
        }

        public IWriteIndex Index(string name)
        {
            _sb.AppendFormat(" index {0}", _provider.EscapeIdentifier(name));
            return this;
        }

        public IWriteIndex Unique()
        {
            _sb.Append(" unique");
            return this;
        }

        public IWriteIndex OnColumns(IEnumerable<ColumnInfo> columns)
        {
            var tbl = columns.First().Table;
            _sb.AppendFormat(" on {0}", tbl.EscapeName(_provider));
            _sb.WriteColumns(columns,_provider);
            
            return this;
        }
    }
}