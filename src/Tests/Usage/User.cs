using System;

namespace Tests.Usage
{
	public class User
	{
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public bool IsDeleted { get; set; }
		public int Ignored { get; set; }

		public int Posts { get; set; }
		public DateTime CreatedOn { get; set; } = DateTime.Now;
		public ArticleType Category { get; set; }/* = Type.Post.ToString();*/
		public User()
		{

		}
	}

	public enum ArticleType
	{
		Post,
		Page
	}
}