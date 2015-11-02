using System;

namespace SqlFu.Builders
{
    public interface IBuildQueryFrom
    {
        IWhere<T> From<T>(Action<IHelperOptions> opt = null);
        IWhere<T> From<T>(T anon, Action<IHelperOptions> options) where T : class;
    }
}