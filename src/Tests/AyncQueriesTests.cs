using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Diagnostics;
using SqlFu;

namespace Tests.SqlFu45
{
    public class AyncQueriesTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        

        public AyncQueriesTests()
        {
            Config.EnsurePosts();
            
        }

        [Fact]
        public async Task PagedQuery()
        {
            using (var db = Config.GetDb())
            {
                var t = await db.PagedQueryAsync<Post>(2, 7, "select * from Posts");
                Assert.Equal(20, t.Count);
                Assert.Equal(7,t.Items.Count());    
            }
            
        }

        [Fact]
        public async Task GetValue()
        {
            using (var db = Config.GetDb())
            {
                using (var t = db.BeginTransaction())
                {
                   var count = await db.GetValueAsync<int>("select count(*) from Posts");
                    Assert.Equal(20, count);
                }
                
            }
            
        }

        [Fact]
        public async Task GetFirstRow()
        {
            using (var db = Config.GetDb())
            {
                var post = await db.GetAsync<Post>(p => p.Id > 12);
                Assert.Equal(13,post.Id);
                Write(post.Id.ToString());
                
            }            
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
            
            Config.CleanUp();
        }
    }
}