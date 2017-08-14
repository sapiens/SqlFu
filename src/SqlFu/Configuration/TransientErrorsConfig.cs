using System;

namespace SqlFu.Configuration
{
    [Obsolete]
    public class TransientErrorsConfig
    {
        /// <summary>
        /// How many times to try the operation.Default is 10
        /// </summary>
        public int Tries=10;
        /// <summary>
        /// Period in ms to wait between tries. Default is 50
        /// </summary>
        public int Wait=50;
    }
}