using CavemanTools.Model;
using CavemanTools.Model.ValueObjects;
using SqlFu;
using Xunit;
using System;
using System.Diagnostics;
using System.Linq;

namespace Tests.Helpers
{
    public class QueryTests:IDisposable
    {
        private Stopwatch _t = new Stopwatch();
        private DbAccess _db;

        public QueryTests()
        {
            _db = Config.GetDb();
        }

        [Fact]
        public void paged_query()
        {
            Config.EnsurePosts();
            var rez=_db.PagedQuery<Post>(1, 3, "select Id,Title,34 as ty from Posts where id<@id order by id desc",new{id=7});
            Assert.Equal(6,rez.Count);
            Assert.Equal(3,rez.Items.Count());
            Assert.Equal(5,rez.Items.First().Id);
            Assert.Equal(4,rez.Items.Skip(1).First().Id);
            Assert.Equal(3,rez.Items.Skip(2).First().Id);
        }

        [Fact]
        public void query_list_param()
        {
            
            Config.EnsurePosts();
            var list = new[] {1, 2, 3};
            var all=_db.Query<Post>("select * from posts where Title=@1 or id in (@0)", list,"fer").ToArray();
            Assert.Equal(3,all.Length);
        }

        [Fact]
        public void working_with_enums_as_string()
        {
            var pst=_db.Query<Post>("select 'Page' as Type").First();
            Assert.Equal(PostType.Page,pst.Type);
        }

        [Fact]
        public void working_with_enums_as_int()
        {
            
            var pst = _db.Query<Post>("select 1 as Type").First();
            Assert.Equal(PostType.Page, pst.Type);
          
        }

        [Fact]
        public void working_with_nullables()
        {

            var pst = _db.Query<Post>("select 1 as id, 1 as TopicId").First();
            Assert.Equal(1, pst.TopicId);
            
            pst = _db.Query<Post>("select 1 as id, null as TopicId").First();
            Assert.Null(pst.TopicId);

            Assert.Equal(0,_db.GetValue<int?>("select 0"));
            Assert.Null(_db.GetValue<int?>("select null"));
        }

        [Fact]
        public void query_list_dynamic()
        {

            Config.EnsurePosts();
            var list = new[] { 1, 2, 3 };
            var all = _db.Query<dynamic>("select * from posts where Title=@1 or id in (@0)", list, "fer").ToArray();
            Assert.Equal(3, all.Length);
        }


        [Fact]
        public void apply_to_command()
        {
            var before = false;
            _db.WithSql("select 1").ApplyToCommand(d =>before = true).ExecuteQuery<Post>().First();
            Assert.True(before);
        }

        [Fact]
        public void custom_mapper()
        {
            var custom = false;
            var d= _db.WithSql("select 1 as Id, 'test' as Name").ExecuteQuery(rd =>
                                                                           {
                                                                               var rez = new IdName();
                                                                               rez.Id = rd.GetInt32(0);
                                                                               rez.Name = rd.GetString(1);
                                                                               custom = true;
                                                                               return rez;
                                                                           }).First();
            Assert.True(custom);
            Assert.Equal(1,d.Id);
            Assert.Equal("test",d.Name);
        }

        [Fact]
        public void set_global_mapper_per_type()
        {
            var custom = false;
            PocoFactory.RegisterMapperFor<IdName>(rd=>
                                                      {
                                                          custom = true;
                                                          return null;
                                                      });
            Assert.Null(_db.Query<IdName>("select 1").First());
            Assert.True(custom);
        }

        [Fact]
        public void custom_value_converter()
        {
            var c = false;
            _db.WithSql("select 1").ExecuteScalar(o =>
                                                      {
                                                          c = true;
                                                          return 1;
                                                      });
            Assert.True(c);
        }

        [Fact]
        public void global_value_converter()
        {
            var c = false;
            PocoFactory.RegisterConverterFor(o=>
                                              {
                                                  c = true;
                                                  return 0;
                                              });
            Assert.Equal(0,_db.GetValue<int>("select 1"));
            Assert.True(c);

            PocoFactory.RegisterConverterFor<Email>(o=> new Email(o.ToString()));
            var em=_db.GetValue<Email>("select 'mike@test.com'");
            Assert.Equal("mike@test.com",em.Value);

        }

        private void Write(string format, params object[] param)
        {
            Console.WriteLine(format, param);
        }

        public void Dispose()
        {
           Config.EmptyTable();
        }
    }
}