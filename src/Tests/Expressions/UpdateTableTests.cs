using System.Linq.Expressions;
using System.Text;
using Moq;
using SqlFu;
using SqlFu.Expressions;
using SqlFu.Providers.SqlServer;
using Xunit;
using System;
using System.Diagnostics;
using System.Linq;

namespace Tests.Expressions
{
    public class UpdateTableTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private StringBuilder _sb;
        private ExpressionWriter _w;
        //private Mock<IDbProviderExpressionHelper> _provider;
        private Expression<Func<Post,object>> _data;
        private UpdateTableBuilder<Post> _builder;

        public UpdateTableTests()
        {
            _sb = new StringBuilder();
            _w = new ExpressionWriter(_sb,new SqlServerBuilderHelper());
            var db = new Mock<IAccessDb>();
            db.Setup(d => d.Provider).Returns(new SqlServerProvider());
            _builder = new UpdateTableBuilder<Post>(db.Object);
        }

        [Fact]
        public void set_title_to_fred()
        {
            _builder.Set(p => p.Title, "Fred");
            Assert.Equal("update [Posts] set [Title]=@0",_builder.GetSql());
            Assert.Equal("Fred",_builder.GetParameters().First());
         
        }

        [Fact]
        public void set_title_where_id()
        {
            _builder.Set(p => p.Title, "Fred").Set("IsActive", false);
            _builder.Where(p => p.Id == 12);
            Assert.Equal("update [Posts] set [Title]=@0, [IsActive]=@1 where ([Id] = 12)",_builder.GetSql());
        }

        [Fact]
        public void set_column_operation()
        {
            var id = 23;
            _builder.Set(p => p.TopicId, p => p.TopicId + 1).Where(p => p.Id == id);
            Assert.Equal("update [Posts] set [TopicId]=([TopicId] + 1) where ([Id] = @0)",_builder.GetSql());
            
        }
       
        [Fact]
        public void set_multiple_columns_with_operation()
        {
            var id = 23;
            _builder.Set(p => p.TopicId, p => p.TopicId + 1)
                .Set("IsActive",false);
            _builder.Set(p=>p.Type,PostType.Page)
                .Where(p => p.Id == id);
            Assert.Equal("update [Posts] set [TopicId]=([TopicId] + 1), [IsActive]=@0, [Type]=@1 where ([Id] = @2)",_builder.GetSql());
            
        }

        [Fact]
        public void where_topic_id_is_null_is_written_correctly()
        {
            _builder.Set(d => d.Title, "f").Where(d => d.TopicId == null);
            Assert.Equal("update [Posts] set [Title]=@0 where ([TopicId] is null)",_builder.GetSql());
        }


        [Fact]
        public void data_spike()
        {
            var db = Config.GetDb();
            var p = new Post();
            p.Title = "bla";
            p.IsActive = true;
            p.Type = PostType.Post;
            using (var t = db.BeginTransaction())
            {
                if (!db.TableExists<Post>())
                {
                    db.CreateTable<Post>();
                }
                try
                {

                    var id = db.Insert(p).InsertedId<int>();
                    var builder = db.Update<Post>();
                    builder.Set(pst => pst.Title, v => v.Title.Substring(0, 1)).Where(d => d.Id == id);
                    builder.Execute();
                    var post = db.Get<Post>(ps => ps.Id == id);
                    Assert.Equal("b", post.Title);
                }
                finally
                {
                    db.Drop<Post>();
                }
                t.Commit();
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
            Write(_builder.GetSql());

        }
    }

  
}