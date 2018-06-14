using System;

namespace SqlFu.Configuration.Internals
{
    public interface IConfigurePropertyInfo<R>
    {
        IConfigurePropertyInfo<R> IgnoreAtWriting();
        /// <summary>
        /// Used by Insert helper to retrieve inserted id.
        /// It will be automatically ignored when inserting
        /// </summary>
        /// <returns></returns>
        IConfigurePropertyInfo<R> IsAutoincremented();
        IConfigurePropertyInfo<R> BeforeWritingUseConverter(Func<R,object> writer);
        IConfigurePropertyInfo<R> MapToColumn(string name);
    }
}