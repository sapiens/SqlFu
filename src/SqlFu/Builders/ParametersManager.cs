using System.Collections.Generic;

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
            _params.AddRange(value);
            return this;
        }       


    }
}