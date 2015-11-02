using System;
using System.Data;

namespace SqlFu.Mapping
{
    public interface IRegisterCustomMappers
    {
        void Register<T>(Func<IDataReader, T> mapper);
    }
}