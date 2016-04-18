using CavemanTools.Model;
using SqlFu.Builders;

namespace SqlFu.Providers
{
    public interface IDbProviderHelpers
    {

        /// <summary>
        /// Adds sql to return newly inserted id for the Insert helper
        /// </summary>
        /// <param name="sqlValues">String containing the values to insert e.g: values (@0,@1)</param>
        /// <param name="identityColumn"></param>
        /// <returns></returns>
        string AddReturnInsertValue(string sqlValues, string identityColumn);

        /// <summary>
        /// Adds the pagination syntax to sql
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="page"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        string FormatQueryPagination(string sql, Pagination page, ParametersManager pm);
    }
}