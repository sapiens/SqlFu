namespace SqlFu.Builders.Internals.SqlParts
{
    public class SqlBuilderPartValue:BaseSqlBuilderPart
    {
        private readonly string _value;

        public SqlBuilderPartValue(string partId,string value):base(partId)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}