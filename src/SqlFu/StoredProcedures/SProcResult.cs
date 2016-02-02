using System.Collections.Generic;
using System.Dynamic;

namespace SqlFu
{
    public class SProcResult
    {
        /// <summary>
        /// Values of the output parameters
        /// </summary>
        public dynamic OutputValues { get; private set; } = new ExpandoObject();

        public int ReturnValue { get; internal set; }
    }

    public class SProcResult<T> : SProcResult
    {
        public List<T> Result { get; internal set; } = new List<T>();
    }
}