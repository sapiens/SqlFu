
namespace SqlFu.Builders
{
    public class PagedBuilderResult
    {
        public string CountSql { get; set; }
        public string PagedSql { get; set; }

        public object[] Args { get; set; }
    }
}