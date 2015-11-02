using System;
using System.Collections.Generic;
using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public class ColumnProviderFeatures
    {
        public class FeaturesItem:IConfigureColumnProviderFeatures
        {
            private readonly ColumnProviderFeatures _parent;

            public FeaturesItem(ColumnProviderFeatures parent)
            {
                _parent = parent;
                Options=new List<DbSpecificOption>();
            }

            List<DbSpecificOption> Options { get; set; }
            public string DbType { get; set; }
            public string Redefined { get; set; }
            public IConfigureColumnProviderFeatures UseDbType(string columnType)
            {
                DbType = columnType;
                return this;
            }

            IConfigureColumnProviderFeatures IConfigureColumnProviderFeatures.Redefine(string definition)
            {
                definition.MustNotBeEmpty();
                Redefined = definition;
                return this;
            }

            public IConfigureColumnProviderFeatures AddOptions(DbSpecificOption[] options)
            {
                Options.AddRange(options);
                return this;
            }

            public IConfigureColumnProviderFeatures ForProvider(string providerId)
            {
                return _parent[providerId];
            }
        }
        
        Dictionary<string,FeaturesItem> _providers=new Dictionary<string, FeaturesItem>();
        public FeaturesItem this[string providerId]
        {
            get
            {
                var item = _providers.GetValueOrDefault(providerId);
                if (item == null)
                {
                    item = new FeaturesItem(this);
                    _providers[providerId] = item;
                }
                return item;
            }
        }
    }
}