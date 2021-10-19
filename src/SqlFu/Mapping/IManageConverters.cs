using System;

namespace SqlFu.Mapping
{
    public interface IManageConverters
    {
        T Convert<T>(object o);
        bool HasReadConverter(Type type);

        /// <summary>
        /// Converts value (before it's written to db) if converters had been specified
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        object ProcessBeforeWriting(object value);
        /// <summary>
        /// Converts the read value if it finds a converter
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        object ProcessReadValue(Type propertyType, object value);
	}
}