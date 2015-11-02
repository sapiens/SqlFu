using System.Data;

namespace SqlFu.Mapping
{
    public interface IMapReaderToPoco<T>
    {
        T Map(IDataReader reader, string parentPrefix = ""); 
    }

    public interface IMapToPoco
    {
        T Map<T>(IDataReader reader, string queryId,string parentPrefix = "");
    }
}