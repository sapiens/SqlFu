using SqlFu.Providers;

namespace SqlFu.Configuration
{
    public class SqlProfile
    {
        public string Name { get; set; }
        public IDbProvider Provider { get; set; }
        public string ConnectionString { get; set; }
    }
}