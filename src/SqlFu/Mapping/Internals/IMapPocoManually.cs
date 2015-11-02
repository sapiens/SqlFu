using System;
using System.Data;

namespace SqlFu.Mapping.Internals
{
    public interface IMapPocoManually
    {
        bool HasMapper(Type tp);
        T Map<T>(IDataReader reader);
    }
}