using System;

namespace SqlFu.Configuration.Internals
{
    public class PrimaryKey:BaseConstraint<IConfigurePrimaryKey>,IConfigurePrimaryKey
    {
      //  public bool Auto { get; set; }
      


        IConfigurePrimaryKey IConfigurePrimaryKey.Named(string name)
        {
            name.MustNotBeEmpty();
            Name = name;
            return this;
        }

        //public IConfigurePrimaryKey AutoIncrement(bool auto = true)
        //{
        //    Auto = auto;
        //    return this;
        //}
        //IConfigurePrimaryKey IConfigureProviderOptions<IConfigurePrimaryKey>.ProviderOptions(string providerId, params object[] options)
        //{
        //    return base.ProviderOptions(providerId, options);
        //}
    }
}