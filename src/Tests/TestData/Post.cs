using System;
using CavemanTools.Model;
using CavemanTools.Model.ValueObjects;
using SqlFu;

namespace Tests.TestData
{
    [Table("SomePost",IdentityColumn = "SomeId")]
    public class Post
    {
        public int SomeId { get; set; }
        public Guid Id { get; set; }
        public string Title { get; protected set; }
        public IdName Author { get; set; }
        public string Email { get; set; }
        public DateTime CreatedOn { get; set; }

    }
}