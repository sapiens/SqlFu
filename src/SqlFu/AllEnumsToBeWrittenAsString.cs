namespace SqlFu
{
	/// <summary>
	/// Rule that can be registered
	/// </summary>
	public class AllEnumsToBeWrittenAsString : AWriteConverterRule
	{
		public AllEnumsToBeWrittenAsString() : base(t=>t.IsEnum, e=>e.ToString())
		{
		}
	}
}