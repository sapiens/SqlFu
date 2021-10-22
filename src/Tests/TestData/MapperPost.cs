namespace Tests.TestData
{
	public class MapperPost : Post
	{
		public decimal Decimal { get; set; }
		public Address Address { get; set; }
		public dynamic Dyno { get; set; }

		public byte[] Version { get; set; }
		public SomeEnum? Order { get; set; }
		public bool IsActive { get; set; }
		public bool? IsBla { get; set; }

	}

	public enum SomeEnum
	{
		None, First, Last
	}
}