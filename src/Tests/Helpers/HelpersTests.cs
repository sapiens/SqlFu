using SqlFu;
using Xunit;
using System;
using System.Diagnostics;


namespace Tests.Helpers
{
    public class HelpersTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private SqlFuConnection _db;

      
        public HelpersTests()
        {
            _db = Config.GetDb();
          
          Config.EnsurePosts();
        }

        [Fact]
        public void insert_sql()
        {
            Config.EmptyTable();
            var id = _db.Insert(new Post { Title = "Test", AuthorId = 1, TopicId = null, Type = PostType.Post, CreatedOn = DateTime.UtcNow }).InsertedId<int>();
            Assert.Equal(1, id);
            id = _db.Insert(new Post { Title = "Test", AuthorId = 1, TopicId = null, Type = PostType.Post, CreatedOn = DateTime.UtcNow }).InsertedId<int>();
            Assert.Equal(2, id);

        }

        [Fact]
        public void update_table_poco()
        {
            
            var rez=_db.Update<Post>(new {Title = "alt title", TopicId = 3, Id = 2});
            Assert.Equal(1,rez);
            var p=_db.Get<Post>(2);
            Assert.Equal("alt title",p.Title);
            Assert.Equal(3,p.TopicId);
        }

        [Fact]
        public void update_with_expression_condition_ignores_queryonly_properties()
        {
            _db.Update<Post>(new {Title = "updated",IgnoreWhenUpdate=78},p=>p.Id==2);
            var post = _db.Get<Post>(2);
            Assert.Equal(0,post.IgnoreWhenUpdate);
        }

        [Fact]
        public void update_table_by_name()
        {
            
            var rez = _db.Update("Posts",new { Title = "alt title", TopicId = 3, Id = 2 },4);
            Assert.Equal(1, rez);
            var p = _db.Get<Post>(4);
            Assert.Equal("alt title", p.Title);
            Assert.Equal(3, p.TopicId);
        }

        [Fact]
        public void delete_post()
        {
            Config.EnsurePosts();
            Assert.Equal(1,_db.DeleteFrom<Post>("id=@0", 4));
            Assert.Null(_db.Get<Post>(4));
          //  Assert.Equal(9,_db.ExecuteScalar<int>("select count(*) from posts"));
            
        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        public void Dispose()
        {
           _db.Dispose();
           Config.EmptyTable();
        }
    }


}