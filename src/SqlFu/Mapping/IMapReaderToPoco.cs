using System.Data;
using System.Data.Common;

namespace SqlFu.Mapping
{
    public interface IMapReaderToPoco<T>
    {
        T Map(DbDataReader reader, string parentPrefix = ""); 
    }

    public interface IMapToPoco
    {
        T Map<T>(DbDataReader reader, string queryId,string parentPrefix = "");
    }
}