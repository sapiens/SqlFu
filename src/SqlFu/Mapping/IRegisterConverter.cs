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

        void MapValueObject<T>(Func<T, object> from, Func<object, T> to = null);
    }
}