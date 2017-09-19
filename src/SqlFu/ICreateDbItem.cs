using SqlFu.Configuration;

namespace SqlFu
{
    public interface ICreateDbItem
    {
        TableName Name { get; }
        string Sql { get; }
    }
}