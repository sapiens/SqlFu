using System;
using System.Text;

namespace SqlFu.Builders
{
    public interface IGenerateSql
    {
       CommandConfiguration GetCommandConfiguration();   
    }

    public interface IGenerateSql<T> : IGenerateSql
    {
        IGenerateSql<TProjection> MapTo<TProjection>();
    }

    public class RawSqlString
    {
        private string _d;

        public RawSqlString(string d)
        {
            d.MustNotBeNull();
            this._d = d;
        }

        //public static implicit operator string(RawSqlString d) => d.Value;
        public static implicit operator RawSqlString(string d) => new RawSqlString(d);

        public override string ToString() => _d;

    }
    public class SqlStringBuilder : IGenerateSql
    {
        StringBuilder _sql=new StringBuilder();
        ParametersManager _pm=new ParametersManager();
        public SqlStringBuilder()
        {
            
        }
        public SqlStringBuilder Append(FormattableString data)
        {
            var s = data.Format;

            for (var i = 0; i < data.ArgumentCount; i++)
            {
                s = s.Replace("{" + i + "}", "@" + _pm.CurrentIndex);
                _pm.AddValues(data.GetArgument(i));
            }
            _sql.Append(s);
            return this;
        }

      public SqlStringBuilder AppendRaw(string data)
        {
            _sql.Append(data);
            return this;
        }

        public SqlStringBuilder AppendIf(Func<bool> condition,FormattableString data)
        {
            if (condition()) return Append(data);
            return this;
        }



        public CommandConfiguration GetCommandConfiguration()
        =>new CommandConfiguration(_sql.ToString(),_pm.ToArray());
        
    }
}