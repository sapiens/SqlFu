using System;

namespace SqlFu
{
    public class CreateDbItemBody
    {
        public string Value { get; }

        public CreateDbItemBody(string value)
        {
            var idx = value.IndexOf('(');
            idx.MustBeGreaterThan0();
            Value = value.Substring(idx);
        }

        public static implicit operator string(CreateDbItemBody d) => d.Value;
        public static implicit operator CreateDbItemBody(string d) => new CreateDbItemBody(d);


    }
}