using System;
using CavemanTools.Model;
using CavemanTools.Model.ValueObjects;

namespace SqlFu.Tests._Fakes
{
    [Table("SomePost",IdentityColumn = "SomeId")]
    public class Post
    {
        public int SomeId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; protected set; }
        public IdName Author { get; set; }
        public Email Email { get; set; }


    }
}