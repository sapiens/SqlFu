using SqlFu.Providers;

namespace SqlFu.Configuration
{
    public interface IConfigureColumnProviderFeatures
    {
        IConfigureColumnProviderFeatures UseDbType(string columnType);
        /// <summary>
        /// Use this string for column definition
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        IConfigureColumnProviderFeatures Redefine(string definition);
        IConfigureColumnProviderFeatures AddOptions(DbSpecificOption[] options);
        IConfigureColumnProviderFeatures ForProvider(string providerId);
    }

}