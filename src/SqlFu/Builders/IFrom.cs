using System;

namespace SqlFu.Builders
{
    public interface IBuildQueryFrom
    {
        IWhere<T> From<T>(Action<IHelperOptions> opt = null);
        /// <summary>
        /// Specify the anonymous type to be used as a table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="anon"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        IWhere<T> FromAnonymous<T>(T anon, Action<IHelperOptions> options) where T : class;
    }
}