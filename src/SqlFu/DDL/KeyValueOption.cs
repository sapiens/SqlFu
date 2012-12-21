namespace SqlFu.DDL
{
    public class KeyValueOption : DbSpecificOption
    {
        public KeyValueOption(string name, object value, DbEngine engine) : base(name, engine)
        {
            Symbol = " ";
            Value = value;
        }

        /// <summary>
        /// Default is 1 space " " 
        /// </summary>
        public string Symbol { get; set; }

        public override string ToString()
        {
            return Name + Symbol + Value;
        }
    }
}