using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration.Internals;

namespace SqlFu.Builders.Crud
{
    public class SelectOptions<T> : HelperOptions,IIgnoreSelectColumns<T> where T : class
    {
        public SelectOptions(TableInfo info) : base(info)
        {
        }

        public string[] GetSelectColumns() => Info.Columns
            .Where(d => !IgnoreProperties.Any(p => p == d.PropertyInfo.Name))
            .Select(d => d.Name).ToArray();

        //string[] GetIgnoredColumns() => IgnoreProperties.Select(d => Info[d].Name).ToArray();

        public void Ignore(params Expression<Func<T, object>>[] ignore)
        {
            IgnoreProperties.AddRange(ignore.GetNames()); 
        }
    }
}