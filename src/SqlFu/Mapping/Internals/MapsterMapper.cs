using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Mapster;
using SqlFu.Configuration.Internals;

namespace SqlFu.Mapping.Internals
{
	public class MapsterMapper<T> : IMapReaderToPoco<T>
	{
		private readonly TableInfo _info;
		public MapsterMapper(TableInfo info)
        {
           
        
			_info = info;
		
        
        }
        object _sync = new object();

        Dictionary<string, int> _columnIndexes;
        public T Map(DbDataReader reader, string parentPrefix = "")
		{
			lock (_sync)
			{
                if (_columnIndexes == null) _columnIndexes = MapperFactory.MapColumnIndexesToPoco(_info, reader);
            }

            var temp = DicFromReader(reader);
            return temp.Adapt<T>();
		}

        private Dictionary<string, object> DicFromReader(DbDataReader rd)
        {
            var values = new object[rd.FieldCount];
            rd.GetValues(values);
            var r = new Dictionary<string, object>();
            foreach (var kv in _columnIndexes) r.Add(kv.Key, _info.ConvertReadValue(kv.Key,values[kv.Value]));
            return r;
        }
    }

    
}