using SqlFu.Configuration;
using SqlFu.Providers;

namespace Tests._Fakes
{
    public class FakeEscapeIdentifier:IEscapeIdentifier
    {
        public static FakeEscapeIdentifier Instance= new FakeEscapeIdentifier();
        public string EscapeIdentifier(string name) => name;


        public string EscapeTableName(TableName table) => table.Name;

    }
}