using System;
using SqlFu.Configuration;

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
        /// <param name="tableName"></param>
        /// <returns></returns>
        IWhere<T> FromAnonymous<T>(T anon, TableName tableName) where T : class;
    }
}