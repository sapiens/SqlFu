using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Builders.Expressions;
using SqlFu.Configuration;

namespace SqlFu.Builders.CreateTable
{
    public interface IConfigurePrimaryKey<T>
    {
        string Name { get; set; }
        string Options { get; set; }

        void OnColumns(params Expression<Func<T, object>>[] columns);
    }

    public class PKData
    {
        public string Name { get; set; }
        public string[] Columns { get; set; }

        public string Options { get; set; }
    }

    public class PKData<T>:PKData,IConfigurePrimaryKey<T>
    {
        public void OnColumns(params Expression<Func<T, object>>[] columns)
        {
            Columns = columns.GetNames().ToArray();
        }
    }
}