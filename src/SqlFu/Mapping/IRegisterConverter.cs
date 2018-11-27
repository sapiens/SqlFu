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

        void RegisterWriteConverter<T>(Func<T, object> writeToDb);
    }
}