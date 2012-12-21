using SqlFu.Providers.SqlServer;

namespace SqlFu.DDL.Generators.SqlServer.Options
{
    public class IndexIncludeOption : DbSpecificOption
    {
        public IndexIncludeOption(params string[] columns) : base(SqlServerOptions.Include, DbEngine.SqlServer)
        {
            Value = string.Join(",", columns);
        }

        public override string ToString()
        {
            return string.Format("INCLUDE ({0})",
                                 CommonDDLWriter.GetEscapedNames(Value.ToString(), SqlServerProvider.EscapeIdentifier));
        }
    }
}