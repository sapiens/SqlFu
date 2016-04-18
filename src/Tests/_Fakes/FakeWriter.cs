using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using SqlFu.Builders;
using SqlFu.Builders.Expressions;

namespace Tests._Fakes
{
    public class FakeWriter : IGenerateSqlFromExpressions
    {
        Dictionary<string,Queue<string>> _results=new Dictionary<string, Queue<string>>();

        public FakeWriter()
        {
            _results["col"]=new Queue<string>();
            _results["sql"]=new Queue<string>();
        }

        public void SetColumnsResults(params string[] vals )
        {
            vals.ForEach(v=> _results["col"].Enqueue(v));            
        }
        public void SetSqlResults(params string[] vals )
        {
            vals.ForEach(v=> _results["sql"].Enqueue(v));            
        }

        public ParametersManager Parameters { get; }= new ParametersManager();
        public string GetColumnsSql(params Expression[] columns)
        {
            var sb=new StringBuilder();
            columns.ForEach(d=>sb.Append($"{_results["col"].Dequeue()},"));
            return sb.RemoveLast().ToString();
        }

        public string GetSql(Expression expression)
        {
            return _results["sql"].Dequeue();
        }
    }
}