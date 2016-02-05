using System;
using Tests.Mocks;
using Tests._Fakes;

namespace Tests.Providers
{
   
    public class SomePost
    {
        public int Id { get; set; }
        public Guid UId { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }
        public SomeEnum State { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class Category
    {
        public string Name { get; set; }
        public int PostId { get; set; }
    }

    public class SomePostProjection
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}