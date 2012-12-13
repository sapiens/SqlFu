using System;
using CavemanTools.Model;
using ServiceStack.DataAnnotations;
using SqlFu;

namespace Tests
{
    public enum PostType
    {
        Post,
        Page
    }
    [Table("sfPosts")]
    [PetaPoco.PrimaryKey("Id", autoIncrement = true)]
    class sfPosts
    {
      [AutoIncrement]
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        //[InsertAsString]        
        //public PostType Type { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }

        public static sfPosts Create()
        {
            var p = new sfPosts();
            p.AuthorId = 2;
            p.Title="123gf";
            p.CreatedOn = DateTime.UtcNow;
         //   p.Type = PostType.Page;
            p.IsActive = true;
            return p;
        }
    }


    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public IdName Author { get; set; }
       
    }

}