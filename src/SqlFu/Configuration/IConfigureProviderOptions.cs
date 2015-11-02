using SqlFu.Providers;

namespace SqlFu.Configuration
{
    public interface IConfigureProviderOptions<out T>
    {
        T ProviderOptions(string providerId, params DbSpecificOption[] options);
    }
}