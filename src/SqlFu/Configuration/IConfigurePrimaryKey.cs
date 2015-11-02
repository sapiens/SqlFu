
namespace SqlFu.Configuration
{
    public interface IConfigurePrimaryKey:IConfigureProviderOptions<IConfigurePrimaryKey>
    {
        IConfigurePrimaryKey Named(string name);      
    }
}