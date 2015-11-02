namespace SqlFu.Configuration
{
    public interface IConfigureIndex:IConfigureProviderOptions<IConfigureIndex>
    {
        IConfigureIndex Named(string name);
        IConfigureIndex Unique();
       
    }
}