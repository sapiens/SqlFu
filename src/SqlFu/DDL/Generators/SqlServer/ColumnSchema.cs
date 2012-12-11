namespace SqlFu.DDL.Generators.SqlServer
{
    internal class ColumnSchema
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public string CharacterMaximumLength { get; set; }
        public string NumericPrecision { get; set; }
        public string NumericScale { get; set; }
        public string CollationName { get; set; }
    }
}