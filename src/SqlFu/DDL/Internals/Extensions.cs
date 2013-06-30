using System.Text.RegularExpressions;

namespace SqlFu.DDL.Internals
{
    internal static class Extensions
    {
        /// <summary>
        /// Removes characters used to escape identifiers.
        /// Clears out [] " ` .
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string FilterEscape(this string name)
        {
            return Regex.Replace(name, @"[\[\]\.`""]", "");
        }
    }
}