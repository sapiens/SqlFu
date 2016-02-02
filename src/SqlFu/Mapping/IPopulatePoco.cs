using System.Data;
using System.Data.Common;

namespace SqlFu.Mapping
{
    public interface IPopulatePoco<T>
    {
        void Populate(T poco, DbDataReader reader);
    }
}