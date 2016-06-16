using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CavemanTools.Logging;
using SqlFu.Configuration.Internals;

namespace SqlFu.Mapping.Internals
{
    public class Mapper<T> : IMapReaderToPoco<T>
    {
       
        public Mapper(TableInfo info, IMapToPoco customMapper, string queryId)
        {
            _converter = info.Converter;
            _customMapper = customMapper;
            _queryId = queryId;
            _columns = info.Columns.ToArray();
        }

      
      
        object _sync=new object();
      
        public T Map(DbDataReader reader, string parentPrefix="")
        {
            lock (_sync)
            {
                var idx = Indexes(parentPrefix);
                if (idx==null)
                {
                    this.LogDebug($"Configuring mapping indexes for '{parentPrefix}'");
                    ConfigureIndexes(reader, parentPrefix);
                }
            }            
            var result = (T)typeof (T).GetFactory()();
   
           Populate(result, reader,parentPrefix);
            return result;
        }

        const string EmptyPrefix = "#";

        private void ConfigureIndexes(DbDataReader reader,string prefix)
        {
            var idx = InitIndexes(prefix);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                var name = reader.GetName(i);
                var colIdx = Array.Find(_columns, s => (prefix+s.Name).ToUpper() == name.ToUpper());
                if (colIdx !=null)
                {
                    idx[colIdx.PocoIdx] = i;
                    this.LogDebug($"Property {prefix}_{colIdx.Name} is mapped from reader[{i}]");
                }
            }
        }

        Dictionary<string,int[]> _indexes=new Dictionary<string, int[]>();

        private int[] Indexes(string prefix) => _indexes.GetValueOrDefault(prefix.IsNullOrEmpty()?EmptyPrefix:prefix);

        int[] InitIndexes(string prefix)
        {
            if (prefix.IsNullOrEmpty()) prefix = EmptyPrefix;
            var idx = Enumerable.Repeat(-1, _columns.Length).ToArray();
            _indexes[prefix] = idx;
            return idx;
        }

        Dictionary<string, Action<T, DbDataReader, IManageConverters, IMapToPoco>>_mappers=new Dictionary<string, Action<T, DbDataReader, IManageConverters, IMapToPoco>>();

        void Populate(T poco, DbDataReader reader, string parentPrefix)
        {
            Action<T, DbDataReader, IManageConverters, IMapToPoco> mapper = null;
            var prefix = parentPrefix.IsNullOrEmpty() ? EmptyPrefix : parentPrefix;
            lock (_sync)
            {
                mapper = _mappers.GetValueOrDefault(prefix);
            }
            if (mapper == null)
            {
                int[] idx = null;
                lock (_sync)
                {
                    idx = Indexes(parentPrefix);
                }
                var populator = new PopulatePocoGenerator<T>();
                var items = new List<Expression>();
                items.AddRange(populator.ReadValuesIntoArray(reader.FieldCount));
                for (var i = 0; i < _columns.Length; i++)
                {
                    var column = _columns[i];

                    if (CanBeMappedFromValue(column) && idx[i] == -1) continue;

                    var i1 = idx[i];

                    //one value to map
                    if (CanBeMappedFromValue(column))
                    {
                        var justCast = column.Type.IsAssignableFrom(reader.GetFieldType(i1)) || column.Type==typeof(object);
                        items.Add(populator.AssignPropertyValue(column, i1, justCast));
                        continue;
                    }

                items.Add(populator.AssignCustomMappedValue(column, parentPrefix, _queryId));

                }
                
                var body = Expression.Block(new[] {populator.ValuesVar},items);
                
                mapper=Expression.Lambda<Action<T, DbDataReader, IManageConverters, IMapToPoco>>
                    (body, populator.PocoExpr, populator.ReaderExpr, populator.Converter, populator.CustomMapExpr)
                    .Compile();
                lock (_sync)
                {
                    _mappers[prefix] = mapper;
                }

            }
            mapper(poco, reader, _converter, _customMapper);
        } 
        
     

        private static bool CanBeMappedFromValue(ColumnInfo column)
        {
            return !column.IsComplex || column.HasConverter;
        }


      
       
        private readonly IManageConverters _converter;
        private readonly IMapToPoco _customMapper;
        private readonly string _queryId;
        private ColumnInfo[] _columns;
    }

    
}