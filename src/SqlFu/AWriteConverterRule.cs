using System;

namespace SqlFu
{
	public abstract class AWriteConverterRule
	{
		private readonly Func<Type, bool> match;
		private readonly Func<object, object> converter;

		protected AWriteConverterRule(Func<Type,bool> match,Func<object,object> converter)
		{
			match.MustNotBeNull();
			converter.MustNotBeNull();
			this.match = match;
			this.converter = converter;
		}

		public Func<object, object> Converter => converter;

		public bool AppliesTo(object value) => value == null ? false : match(value.GetType());

		
	}
}