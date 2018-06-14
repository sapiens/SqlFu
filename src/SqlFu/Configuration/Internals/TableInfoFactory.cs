using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using SqlFu.Mapping;

namespace SqlFu.Configuration.Internals
{
    public class TableInfoFactory : ITableInfoFactory
    {
        private readonly IManageConverters _converter;

        ConcurrentDictionary<Type,TableInfo> _cache=new ConcurrentDictionary<Type, TableInfo>();

        public TableInfoFactory(IManageConverters converter)
        {
            _converter = converter;
        }

        public void Add(TableInfo info)
        {
            _cache[info.Type] = info;
            info.Converter = _converter;
         }

        internal string DefaultSchema { get; set; }

        List<Tuple<Func<Type,bool>,Func<Type,TableName>>> _conventions=new List<Tuple<Func<Type, bool>, Func<Type, TableName>>>();

        public void AddNamingConvention(Func<Type, bool> match, Func<Type, TableName> convention)
        {
            _conventions.Add(new Tuple<Func<Type, bool>, Func<Type, TableName>>(match,convention));
        }


        TableName GetName(Type type)
        {
            var convention = _conventions.Find(d => d.Item1(type));
            if (convention==null) return new TableName(type.Name,DefaultSchema);
            return convention.Item2(type);
        }

        public TableInfo GetInfo(Type pocoType) 
            => _cache.GetValueOrCreate(pocoType, () =>
        {
            var table = GetName(pocoType);
           var info= new TableInfo(pocoType, _converter,table);
            return info;
        });
    }
}