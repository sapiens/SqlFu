using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using SqlFu.Configuration;

namespace SqlFu.Mapping.Internals
{
    public class MapperFactory:IMapToPoco
    {
        private readonly IMapPocoManually _customMapper;
        private readonly ITableInfoFactory _infoFactory;
        private readonly IManageConverters _converters;
       // private TypeBuilder _typeBuilder;

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
            
            if (type == typeof (object))
            {
                return new DynamicMapper() as IMapReaderToPoco<T>;
            }

            if (type.IsValueType() || type == typeof(string) || type==typeof(byte[]))
            {
                return new ValueTypeMapper<T>(_converters);
            }
            return new Mapper<T>(_infoFactory.GetInfo(type),this, queryId);
            
        }
    }
}