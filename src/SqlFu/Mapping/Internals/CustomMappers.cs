using System;
using System.Collections.Generic;
using System.Data;

namespace SqlFu.Mapping.Internals
{
    public class CustomMappers : IRegisterCustomMappers, IMapPocoManually
    {
        Dictionary<Type,object>  _mappers=new Dictionary<Type, object>();

        public void Register<T>(Func<IDataReader, T> mapper)
        {
            mapper.MustNotBeNull();
            _mappers[typeof (T)] = mapper;
        }

        public bool HasMapper(Type tp)
        {
            return _mappers.ContainsKey(tp);
        }

        public T Map<T>(IDataReader reader)
        {
            var func = _mappers[typeof (T)] as Func<IDataReader, T>;
            return func(reader);
        }
    }
}