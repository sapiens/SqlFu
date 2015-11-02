using System.Collections.Generic;
using System.Linq;
using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public class ProviderFeaturesCollection
    {
        Dictionary<string,ProviderFeatures> _items=new Dictionary<string, ProviderFeatures>();

        public List<DbSpecificOption> this[string providerId]
        {
            get
            {
                var list= _items.GetValueOrDefault(providerId);
                if (list == null)
                {
                    list=new ProviderFeatures();
                    _items[providerId] = list;
                }
                return list;
            }            
        }

        public ProviderFeatures GetFeatures(string providerId)
        {
            var rez= _items.GetValueOrDefault(providerId);
            return rez ?? new ProviderFeatures();
        }

    }

    public class ProviderFeatures:List<DbSpecificOption>
    {
        public bool HasOption(string option)
        {
            return this.Any(d => d.Name == option);
        }
    }
}