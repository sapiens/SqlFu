using System;

namespace Tests.SqlServer
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
      
        public bool IsDeleted { get; set; }
        public int Posts { get; set; }
        public DateTime CreatedOn { get; set; }=DateTime.Now;
        public string Category { get; set; } = Type.Post.ToString();
        public User()
        {
         
        }
    }

    public enum Type
    {
        Post,
        Page
    }
}