using SqlFu.Configuration;

namespace SqlFu
{
    public class CreateDbItem : ICreateDbItem
    {
        public TableName Name { get; }
        public string Sql { get; }

        public CreateDbItem(TableName name,CreateDbItemBody sql)
        {
            Name = name;
            Sql =sql;
        }
    }
}