namespace SqlFu.Configuration
{
    public interface IConfigureColumn
    {
        IConfigureColumn Named(string name);
        IConfigureColumn IsNull();
        IConfigureColumn IsNotNull();
        /// <summary>
        /// Is ignored by insert/update
        /// </summary>
        /// <returns></returns>
        IConfigureColumn QueryOnly();

        /// <summary>
        /// Used mainly for enums. 
        /// </summary>
        /// <returns></returns>
        IConfigureColumn StoreAsString();
        IConfigureColumn HasSize(int size);
        /// <summary>
        /// Used for real/float/double/decimal/money types
        /// </summary>
        /// <param name="totalDigits">The maximum total number of decimal digits that will be stored</param>
        /// <param name="digits">The number of decimal digits that will be stored to the right of the decimal point</param>
        /// <returns></returns>
        IConfigureColumn Precision(int totalDigits,int digits);
        IConfigureColumn HasSize(string size);
        IConfigureColumn IsIdentity(int seed=1,int increment=1);
        IConfigureColumn Collation(string collation);
        IConfigureColumn Default_Is_Value(object value);
        IConfigureColumn Default_Is_Function(string value);
        IConfigureColumnProviderFeatures ForProvider(string providerId);

    }
}