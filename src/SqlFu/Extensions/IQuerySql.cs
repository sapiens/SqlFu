using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlFu
{
    public interface IQuerySql<T>
    {
        Task<T> GetValueAsync();
        Task<T> GetFirstRowAsync();
        T GetValue();
        T GetFirstRow();
        List<T> GetRows();
        Task<List<T>> GetRowsAsync();
    }
}