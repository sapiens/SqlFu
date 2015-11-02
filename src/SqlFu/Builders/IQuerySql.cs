using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools.Model;

namespace SqlFu.Builders
{
    public interface IQuerySql<TProjection>:IBuildSql
    {
        Task<IEnumerable<TProjection>> GetRowsAsync(CancellationToken token);
        Task<PagedResult<TProjection>> GetPagedAsync(CancellationToken token);
        Task<PagedResult<TOther>> GetPagedAsync<TOther>(CancellationToken token);
        Task<IEnumerable<TOther>> GetRowsAsync<TOther>(CancellationToken token);
        Task<TProjection> GetValueAsync(CancellationToken token);
        
        IEnumerable<TProjection> GetRows();
        PagedResult<TProjection> GetPaged();
        PagedResult<TOther> GetPaged<TOther>();
        IEnumerable<TOther> GetRows<TOther>();
        TProjection GetValue();
    }
}