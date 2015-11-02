namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartLimit : BaseSqlBuilderPart
    {
        private readonly int _take;
        private readonly long _skip;

        public SqlBuilderPartLimit(int take,long skip) : base(ASqlBuilder.Limit_)
        {
            _take = take;
            _skip = skip;
        }

        public int Take
        {
            get { return _take; }
        }

        public long Skip
        {
            get { return _skip; }
        }
    }
}