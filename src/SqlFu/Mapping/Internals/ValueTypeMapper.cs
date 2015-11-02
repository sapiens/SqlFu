using System.Data;

namespace SqlFu.Mapping.Internals
{
    public class ValueTypeMapper<T> : IMapReaderToPoco<T>
    {
        private readonly IManageConverters _converters;

        public ValueTypeMapper(IManageConverters converters)
        {
            _converters = converters;
        }

        public T Map(IDataReader reader, string parentPrefix = "")
        {
            return _converters.Convert<T>(reader.GetValue(0));
        }
    }
}