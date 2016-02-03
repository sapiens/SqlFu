using System.Data.Common;

namespace SqlFu.Mapping.Internals
{
    public class ValueTypeMapper<T> : IMapReaderToPoco<T>
    {
        private readonly IManageConverters _converters;

        public ValueTypeMapper(IManageConverters converters)
        {
            _converters = converters;
        }

        public T Map(DbDataReader reader, string parentPrefix = "") => _converters.Convert<T>(reader.GetValue(0));
    }
}