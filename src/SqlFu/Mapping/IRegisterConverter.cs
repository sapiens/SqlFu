using System;

namespace SqlFu.Mapping
{
    public interface IRegisterConverter
    {
        /// <summary>
        /// Registers converter from object to specified type. If a converter exists, it replaces it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="converter"></param>
        void RegisterConverter<T>(Func<object, T> converter);

        ///// <summary>
        ///// Registers how to convert a value object (custom object) to a insertable value
        ///// and how to convert a value to that object.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="from">From value object to insertable value</param>
        ///// <param name="to">From column value to value object</param>
        //void MapValueObject<T>(Func<T, object> from, Func<object, T> to = null);
    }
}