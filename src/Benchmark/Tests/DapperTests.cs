using System;
using System.Data.SqlClient;
using System.Linq;
using CavemanTools.Model;
using CavemanTools.Testing;
using Dapper;
using Tests;
using DapperExtensions;

namespace Benchmark.Tests
{
    public class DapperTests : PerformanceTests, IDisposable
    {
        private SqlConnection _cnx;

        public DapperTests()
        {
            _cnx = Config.GetOpenConnection();
        }

        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(id =>
                {
                    _cnx.Get<sfPosts>(3);
                    
                  }, "Dapper get entity");

            bc.Add(id =>
            {
                _cnx.Query<sfPosts>("select Id,AuthorId,Title,CreatedOn,'Post' as Type,TopicId,IsActive from sfPosts where id=@id", new { id = 3 }).FirstOrDefault();

            }, "Dapper query entity");
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {

               _cnx.Query<dynamic>("select Id,AuthorId,Title,CreatedOn,'Post' as Type,TopicId,IsActive from sfPosts where id=@id", new { id = 3 }).FirstOrDefault();


            }, "Dapper query entitty dynamic");
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {

                _cnx.Query<sfPosts>("select top 10 Id,AuthorId,Title,CreatedOn,'Post' as Type,TopicId,IsActive from sfPosts where id>@id", new { id = 3 }).ToArray();


            }, "Dapper ");
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {

                _cnx.Query<dynamic>("select top 10 Id,AuthorId,Title,CreatedOn,'Post' as Type,TopicId,IsActive from sfPosts where id>@id", new { id = 3 }).ToArray();


            }, "Dapper ");
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                throw new NotSupportedException("No implicit pagination support");

            }, "Dapper ");
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _cnx.Query<string>("select title from sfPosts where id=@id", new { id = 3 }).Single();
                
            }, "Dapper scalar int");

            //bc.Add(id =>
            //{
            //    _cnx.Query<int?>("select topicid from sfPosts where id>@id order by id", new { id = 3 }).ToArray();

            //}, "Dapper scalar nullable");
            //bc.Add(id =>
            //{
            //    _cnx.Query<PostType>("select type from sfPosts where id>@id order by id", new { id = 3 }).ToArray();

            //}, "Dapper scalar enum");
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
               _cnx.Query<PostViewModel, IdName,PostViewModel>("select p.*,p.Id ,p.title as Name from sfposts p where p.id=@id order by p.id",(pvm,i)=>
                                                                                                                                                  {
                                                                                                                                                      pvm.Author= new IdName();
                                                                                                                                                      return pvm;
                                                                                                                                                  }
                                                                                                                                            ,new{id=5}).FirstOrDefault();
               
            }, "Dapper ");
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(id =>
            {
                _cnx.Insert(p);

            }, "Dapper");
        }

        public override void Updates(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            p.Id = 3;
            p.Title = "updated";
            bc.Add(id =>
                       {
                           _cnx.Update(p);

                       }, "Dapper ");
        }


        public void Dispose()
        {
            _cnx.Dispose();
        }
    }
}