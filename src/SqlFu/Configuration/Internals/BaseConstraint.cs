using SqlFu.Providers;

namespace SqlFu.Configuration.Internals
{
    public abstract class BaseConstraint<T>:IConfigureProviderOptions<T> where T:class 
    {
        public string Name { get; set; }
        public ColumnInfo[] Columns { get; set; }
        public ProviderFeaturesCollection Features { get; private set; }

        public BaseConstraint()
        {
            Features=new ProviderFeaturesCollection();
            Columns = new ColumnInfo[0];
        }

        public T ProviderOptions(string providerId, params DbSpecificOption[] options)
        {
            Features[providerId].AddRange(options);
            return this as T;
        }
    }
}