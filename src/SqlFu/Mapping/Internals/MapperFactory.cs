using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using SqlFu.Configuration;
using SqlFu.Configuration.Internals;

namespace SqlFu.Mapping.Internals
{
    public class MapperFactory:IMapToPoco
    {
        private readonly IMapPocoManually _customMapper;
        private readonly ITableInfoFactory _infoFactory;
        private readonly IManageConverters _converters;

        public MapperFactory(IMapPocoManually customMapper,ITableInfoFactory infoFactory,IManageConverters converters)
        {
            _customMapper = customMapper;
            _infoFactory = infoFactory;
            _converters = converters;
       
        }

      
        public Dictionary<string, object> Mappers { get; } = new Dictionary<string, object>();

        public T Map<T>(DbDataReader reader, string queryId,string prefix=null)
        {
            if (_customMapper.HasMapper(typeof (T))) return _customMapper.Map<T>(reader);
            queryId.MustNotBeEmpty();
            var key = queryId + typeof (T);
            var mapper = Mappers.GetValueOrDefault(key) as IMapReaderToPoco<T>;
            if (mapper == null)
            {
                mapper = CreateMapper<T>(queryId);
                try
                {
                    Mappers.Add(key, mapper);
                }
                catch (ArgumentException)
                {
                    //we ignore it, a mapper is already available
                }
            }
            return mapper.Map(reader, prefix);
        }

        public IMapReaderToPoco<T> CreateMapper<T>(string queryId)
        {
            var type = typeof (T);
            if (type.CheckIfAnonymousType())
            {
                return new AnonymousTypeMapper<T>();
            }
            
            if (type == typeof (object) || type== typeof(IDictionary<string,object>))
            {
                return new DynamicMapper() as IMapReaderToPoco<T>;
            }

            if (type.IsValueType() || type == typeof(string) || type==typeof(byte[]))
            {
                return new ValueTypeMapper<T>(_converters);
            }
            return new MapsterMapper<T>(_infoFactory.GetInfo(type));
           // return new Mapper<T>(_infoFactory.GetInfo(type),this, queryId);
            
        }

        static internal Dictionary<string, int> MapColumnIndexesToPoco(TableInfo info,DbDataReader reader)
        {
            var idxDic = new Dictionary<string, int>();
             var cols = info.GetColumnNames(IgnoreOn.Read);
            
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i).ToUpper();
                var kv = cols.Where(d => d.Value.ToUpper() == name).FirstOrDefault();


                if (kv.Key != null)
                {
                    idxDic[kv.Key] = i;
                    SqlFuManager.LoggerName.LogDebug($"Property {info.TableName}.{kv.Key} is mapped from reader[{i}]({name})");
                }
            }
            return idxDic;


        }
    }
}