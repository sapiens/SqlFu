using System;

namespace SqlFu.Mapping
{
    public interface IManageConverters
    {
        T Convert<T>(object o);
        bool HasConverter(Type type);

        object ProcessBeforeWriting(object value);
       
    }
}