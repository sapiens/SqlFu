using System.Collections.Generic;

namespace SqlFu
{
    /// <summary>
    /// Contains return value and output parameter values
    /// </summary>
    public class StoredProcedureResult
    {
        public StoredProcedureResult()
        {
            OutputValues = new Dictionary<string, object>();
        }

        public int ReturnValue { get; set; }
        public IDictionary<string, object> OutputValues { get; set; }
    }
}