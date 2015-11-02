using System;
using System.Linq;
using System.Linq.Expressions;
using SqlFu.Configuration;

namespace SqlFu.Builders.CreateTable
{
    public class IndexDefinition
    {
        public string Name { get; set; }
        internal string[] Columns { get; set; }
        public bool IsUnique { get; set; }

        public string Options { get; set; }

        public void SetColumns<T>(params Expression<Func<T, object>>[] columns)
        {
            Columns=columns.Select(c => c.GetPropertyName()).ToArray();
        }
    }

    public class IndexDefinition<T> : IndexDefinition, IConfigureIndex<T>
    {
        public IConfigureIndex<T> WithName(string name)
        {
            Name = name;
            return this;
        }

        public IConfigureIndex<T> Unique()
        {
            IsUnique = true;
            return this;
        }

        public IConfigureIndex<T> WithOptions(string options)
        {
            Options = options;
            return this;
        }

        public IConfigureIndex<T> OnColumns(params Expression<Func<T, object>>[] columns)
        {
            SetColumns(columns);
            return this;
        }
    }

    public interface IConfigureIndex<T>
    {
        IConfigureIndex<T> WithName(string name);
        IConfigureIndex<T> Unique();

        IConfigureIndex<T> WithOptions(string options);

        IConfigureIndex<T> OnColumns(params Expression<Func<T, object>>[] columns);

    }
}