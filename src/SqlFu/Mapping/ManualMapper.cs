using System;
using System.Data;

namespace SqlFu.Mapping
{
    public class ManualMapper<T> : IMapReaderToPoco<T>
    {
        private readonly Func<IDataReader, T> _map;

        public ManualMapper(Func<IDataReader,T> map)
        {
            _map = map;
        }

        public T Map(IDataReader reader, string parentPrefix = "")
        {
            return _map(reader);
        }
    }

    public class ManualMapper
    {
        public static ManualMapper<T> For<T>(T anon, Func<IDataReader, T> map)
        {
            return new ManualMapper<T>(map);
        }

    }

}