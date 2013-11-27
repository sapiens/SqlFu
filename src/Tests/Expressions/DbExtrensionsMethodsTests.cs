using SqlFu;
using Xunit;
using System;
using System.Diagnostics;
using System.Linq;

namespace Tests.Expressions
{
    public class DbExtrensionsMethodsTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private SqlFuConnection _db;

        public DbExtrensionsMethodsTests()
        {
            _db = Config.GetDb();
            Config.EnsurePosts();
        }

        [Fact]
        public void count()
        {
            Assert.Equal(20,_db.Count<Post>());
            Assert.Equal(0,_db.Count<Post>(p=>p.Id>30));
            Assert.Equal(20,_db.Count<Post>(p=>p.TopicId==null));
        }
        
        [Fact]
        public void any()
        {
            Assert.True(_db.HasAnyRows<Post>());
            Assert.False(_db.HasAnyRows<Post>(p=>p.IsActive));
        }

        [Fact]
        public void get_column_value()
        {
            var post = new Post() {Title = "title12"};
            var id = _db.Insert(post).InsertedId<int>();
            Assert.Equal("title12",_db.GetColumnValue<Post,string>(p=>p.Title,p => p.Id == id));
            Assert.Null(_db.GetColumnValue<Post, string>(p => p.Title, p => p.Id == 2890));
        }

        [Fact]
        public void get_entity_by_criteria()
        {
            var post = new Post() { Title = "title12" };
            var id = _db.Insert(post).InsertedId<int>();
            var ent = _db.Get<Post>(p => p.Id == id);
            Assert.Equal("title12",ent.Title);

            ent = _db.Get<Post>(p => p.Id == 280);
            Assert.Null(ent);
        }

        [Fact]
        public void delete_with_criteria()
        {
            var all = _db.Count<Post>();
            _db.DeleteFrom<Post>(p => new[] {1, 2}.Contains(p.Id));
            Assert.Equal(all-2,_db.Count<Post>());
            
        }

        [Fact]
        public void query()
        {
            Assert.Equal(5,_db.Query<Post>(P => P.CreatedOn.Year <= 2014 && P.Id<=5).Count());
        }

        protected void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _db.Drop<Post>();
        }
    }
}