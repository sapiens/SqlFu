using System;
using System.Text;
using SqlFu.Configuration.Internals;
using SqlFu.Providers;

namespace SqlFu.Builders.Internals
{
    class Column : IWriteColumnDefinition
    {
        private readonly ColumnInfo _info;
        private readonly StringBuilder _sb;
        private readonly IDbProvider _provider;

        public Column(ColumnInfo info,StringBuilder sb,IDbProvider provider)
        {
            _info = info;
            _sb = sb;
            _provider = provider;
        }

        public ColumnInfo Info
        {
            get { return _info; }
        }

        public void Dispose()
        {
            _sb.Append(',').AppendLine();
        }

        ColumnProviderFeatures.FeaturesItem Feature()
        {
            return _info.Features[_provider.ProviderId];
        }

        public IWriteColumnDefinition WriteNameAndType()
        {
            _sb.Append(_provider.EscapeIdentifier(Info.Name)).Append(' ');

            if (Info.TreatAsString)
            {
                _sb.Append(_provider.GetColumnType(typeof (string)));
                return this;
            }

            var dbType = Feature().DbType;
            if (dbType.IsNullOrEmpty())
            {
                dbType = _provider.GetColumnType(Info.Type);
                _sb.Append(dbType);               
            }
            else
            {
                _sb.Append(dbType);
            }
            
            return this;
        }

        public IWriteColumnDefinition WriteSize()
        {
            if (Info.Type == typeof(Single))
            {
                _sb.Append("(24)");
                return this;
            }

            if (Info.Type == typeof(Double))
            {
                _sb.Append("(53)");
                return this;
            }

            if (_provider.DbTypeHasPrecision(Info.Type))
            {
                var prec = Info.Precision;
                if (prec != null)
                {
                    _sb.AppendFormat("({0},{1})", prec.Item1, prec.Item2);
                }
                return this;
            }

            if (_provider.DbTypeHasSize(Info.Type))
            {
                var size = Info.Size;
                if (size.IsNullOrEmpty())
                {
                    size = _provider.GetTypeMaxSize(Info.Type);
                }
                if (!size.IsNullOrEmpty())
                {
                    _sb.AppendFormat("({0})", size);
                }
            }
            return this;
        }

        public IWriteColumnDefinition WriteCollation()
        {
            if (!Info.Collation.IsNullOrEmpty())
            {
                _sb.AppendFormat(" COLLATE {0}", Info.Collation);
            }
            return this;
        }

        public IWriteColumnDefinition WriteNull()
        {
            if (!Info.IsNullable)
            {
                _sb.Append(" NOT");
            }

            _sb.Append(" NULL");
            return this;
        }

        public IWriteColumnDefinition WriteDefault()
        {
            if (Info.DefaultValue != null)
            {
                var value = Info.DefaultValue.ToString();
                if (!value.IsNullOrEmpty())
                {
                    ////is not a function
                    // if (!value.Contains("("))
                    // {
                    //     value = "'" + value + "'";
                    // }
                    _sb.AppendFormat(" DEFAULT {0}", value);
                }
            }
            return this;
        }

       
      
    }
}