using System;

namespace SqlFu
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryOnlyAttribute : Attribute
    {
    }
}