using System.Collections.Generic;
using System.Linq;

namespace SqlFu.Builders
{
    public class ParametersManager
    {
        private readonly List<object> _params =new List<object>();

        public ParametersManager()
        {
            
        }
        public ParametersManager(IEnumerable<object> args)
        {
            _params.AddRange(args);
        }

        public object[] ToArray()
        {
            return _params.ToArray();
        }


        /// <summary>
        /// param position
        /// </summary>
        public int CurrentIndex
        {
            get { return _params.Count; }
        }

        public ParametersManager AddValues(params object[] value)
        {
            if (value.Length==1) _params.Add(value[0]);
            else _params.AddRange(value);
            return this;
        }       


    }
}