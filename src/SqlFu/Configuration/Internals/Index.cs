using System;

namespace SqlFu.Configuration.Internals
{
    public class Index: BaseConstraint<IConfigureIndex>, IConfigureIndex
    {
        public bool IsUnique { get; set; }

        public Index()
        {
            
        }

        IConfigureIndex IConfigureIndex.Named(string name)
        {
            name.MustNotBeNull();
            Name = name;
            return this;
        }

        IConfigureIndex IConfigureIndex.Unique()
        {
            IsUnique = true;
            return this;
        }
    
    }
}