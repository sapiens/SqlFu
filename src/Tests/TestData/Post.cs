using System;

namespace Tests.TestData
{
	public class Post
	{
		public int SomeId { get; set; }
		public Guid Id { get; set; }
		public string Title { get; protected set; }
		public IdName Author { get; set; }
		public string Email { get; set; }
		public DateTime CreatedOn { get; set; }
		public DateTimeOffset RegOn { get; set; }
	}

	public record IdName(int Id,string Name);
}