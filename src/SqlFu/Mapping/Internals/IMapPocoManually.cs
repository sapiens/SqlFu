using System;

using System.Data.Common;

namespace SqlFu.Mapping.Internals
{
    public interface IMapPocoManually
    {
        bool HasMapper(Type tp);
        T Map<T>(DbDataReader reader);
    }
}