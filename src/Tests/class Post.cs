using System;
using System.Data;
using CavemanTools.Model;
using SqlFu;
using SqlFu.DDL;

namespace Tests
{
    public enum PostType
    {
        Post,
        Page
    }
    [Table("Posts",CreationOptions = IfTableExists.DropIt)]
    class Post
    {
        public Post()
        {
            CreatedOn = DateTime.Now;
         
        }
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        //[InsertAsString]
        public PostType Type { get; set; }
        public int? TopicId { get; set; }
        public bool IsActive { get; set; }
        [QueryOnly]
        [ColumnOptions(DefaultValue = "0")]
        public int IgnoreWhenUpdate { get; set; }
        
    }


    class OtherPost
    {
        private OtherPost()
        {
           
        }

        public string Name { get; private set; }      
    }


    public class PostViewModel
    {
        public int Id { get; set; }
        public IdName Author { get; set; }
        public string Title { get; set; }
        public DateTime CreatedOn { get; set; }
        public PostType Type { get; set; }
        public IdName Topic { get; set; }
    }

}