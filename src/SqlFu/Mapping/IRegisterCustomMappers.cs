using System;
using System.Data;
using System.Data.Common;

namespace SqlFu.Mapping
{
    public interface IRegisterCustomMappers
    {
        void Register<T>(Func<DbDataReader, T> mapper);
    }
}