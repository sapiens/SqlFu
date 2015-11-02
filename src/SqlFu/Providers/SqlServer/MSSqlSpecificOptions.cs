using SqlFu.Providers;

namespace SqlFu.SqlServer
{
    public class MsSqlSpecificOptions:IMsSqlIndexOptions
    {
        internal const string ClusteredIndex = "CLUSTERED";
        internal const string NonClusteredIndex = "NONCLUSTERED";

        public IMsSqlIndexOptions Index
        {
            get { return this; }
        }

       
        public DbSpecificOption Clustered()
        {
            return new DbSpecificOption(MsSqlProvider.Id,ClusteredIndex);
        }

        public DbSpecificOption NotClustered()
        {
            return new DbSpecificOption(MsSqlProvider.Id, NonClusteredIndex);
        }
    }
}