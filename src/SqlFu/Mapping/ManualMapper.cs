using System;
using System.Data;
using System.Data.Common;

namespace SqlFu.Mapping
{
    public class ManualMapper<T> : IMapReaderToPoco<T>
    {
        private readonly Func<DbDataReader, T> _map;

        public ManualMapper(Func<DbDataReader,T> map)
        {
            _map = map;
        }

        public T Map(DbDataReader reader, string parentPrefix = "")
        {
            return _map(reader);
        }
    }

    public class ManualMapper
    {
        public static ManualMapper<T> For<T>(T anon, Func<DbDataReader, T> map)
        {
            return new ManualMapper<T>(map);
        }

    }

}