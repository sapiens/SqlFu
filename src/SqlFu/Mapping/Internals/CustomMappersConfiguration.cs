using System;
using System.Collections.Generic;
using System.Data.Common;

namespace SqlFu.Mapping.Internals
{
    public class CustomMappersConfiguration : IRegisterCustomMappers, IMapPocoManually
    {
        Dictionary<Type,object>  _mappers=new Dictionary<Type, object>();

        public void Register<T>(Func<DbDataReader, T> mapper)
        {
            mapper.MustNotBeNull();
            _mappers[typeof (T)] = mapper;
        }

        public bool HasMapper(Type tp)
        {
            return _mappers.ContainsKey(tp);
        }

        public T Map<T>(DbDataReader reader)
        {
            var func = _mappers[typeof (T)] as Func<DbDataReader, T>;
            return func(reader);
        }
    }
}