using System.Data;

namespace SqlFu.Mapping
{
    public interface IPopulatePoco<T>
    {
        void Populate(T poco, IDataReader reader);
    }
}