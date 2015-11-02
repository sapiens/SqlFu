using System;
using System.Collections.Generic;
using SqlFu.Builders.CreateTable;
using SqlFu.Mapping;

namespace SqlFu.Configuration.Internals
{
    public class TableInfoFactory : ITableInfoFactory
    {
        private readonly IManageConverters _converter;

        Dictionary<Type,TableInfo> _cache=new Dictionary<Type, TableInfo>();

        public TableInfoFactory(IManageConverters converter)
        {
            _converter = converter;
        }

        public void Add(TableInfo info)
        {
            _cache[info.Type] = info;
            info.Converter = _converter;
         }

        List<Tuple<Func<Type,bool>,Func<Type,TableName>>> _naming=new List<Tuple<Func<Type, bool>, Func<Type, TableName>>>();

        public void AddNamingConvention(Func<Type, bool> match, Func<Type, TableName> convention)
        {
            _naming.Add(new Tuple<Func<Type, bool>, Func<Type, TableName>>(match,convention));
        }


        TableName GetName(Type type)
        {
            var convention = _naming.Find(d => d.Item1(type));
            if (convention==null) return new TableName(type.Name);
            return convention.Item2(type);
        }

        public TableInfo GetInfo(Type pocoType) => _cache.GetValueOrCreate(pocoType, () =>
        {
            var table = GetName(pocoType);
           var info= new TableInfo(pocoType, _converter) {Name = table.Name, DbSchema = table.Schema};
            info.HandleAttributeOverride();
            return info;
        });
    }
}