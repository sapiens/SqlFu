using System;

namespace SqlFu
{
    /// <summary>
    /// Any column with this attribute will be considered a string for insert/update purposes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InsertAsStringAttribute : Attribute
    {
    }
}