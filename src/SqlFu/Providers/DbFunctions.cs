namespace SqlFu.Providers
{
    /// <summary>
    /// Singleton
    /// </summary>
    public class DbFunctions
    {
        public long Count<T>(T column)
        {
            return 0;
        }

        public T Sum<T>(T column)
        {
            return default(T);
        }

        public T Max<T>(T column)
        {
            return default(T);
        }
        
        public T Min<T>(T column)
        {
            return default(T);
        }

        public T Avg<T>(T column)
        {
            return default(T);
        }

        public string Concat(params object[] columns)
        {
            return "";
        }

        public T Round<T>(T data, int decimals)
        {
            return default(T);
        }

        public T Floor<T>(T data)
        {
            return default(T);
        }
        
        public T Ceiling<T>(T data)
        {
            return default(T);
        }
    }
}