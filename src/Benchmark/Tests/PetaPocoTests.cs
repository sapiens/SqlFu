using System;
using CavemanTools.Model;
using CavemanTools.Testing;
using PetaPoco;
using Tests;
using System.Linq;


namespace Benchmark.Tests
{
    public class PetaPocoTests:PerformanceTests
    {
        private Database _db;

        public PetaPocoTests()
        {
            _db = new Database(Config.Connex, Config.SqlServerProviderName);
            _db.EnableAutoSelect = false;
            _db.EnableNamedParams = false;
            _db.OpenSharedConnection();
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
           bc.Add(id=>
                      {
                          _db.Single<sfPosts>("select * from sfPosts where id=@0", 5);
                      },"PetaPoco entity");
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Single<dynamic>("select * from sfPosts where id=@0", 5);
            }, "PetaPoco dynamic");
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Query<sfPosts>("select top 10 * from sfPosts where id>@0",5).ToArray();
            }, "PetaPoco");
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(id =>
                       {
                           _db.Query<dynamic>("select top 10 * from sfPosts where id>@0", 5).ToArray();
                       }, "PetaPoco dynamic");
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Page<sfPosts>(0,10,"select * from sfPosts where id>@0", 5);
            }, "PetaPoco");
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.ExecuteScalar<int>("select AuthorId from sfPosts where id=@0", 5);
            }, "PetaPoco int");
            
            //bc.Add(id =>
            //{
            //   // _db.ExecuteScalar<int?>("select topicid from sfPosts where id=@0", 5);
            //    throw new NotSupportedException();
            //}, "PetaPoco null to nullable"); 
            //bc.Add(id =>
            //{
            //    _db.ExecuteScalar<PostType>("select type from sfPosts where id=@0", 5);
            //}, "PetaPoco enum");
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Query<PostViewModel,IdName>("select p.*,p.Id ,p.title as Name from sfposts p where p.id=@0 order by p.id", 5).FirstOrDefault();                
            }, "PetaPoco");
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(id =>
                       {
                           _db.Insert(p);
                       }, "PetaPoco");
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(id =>
                       {
                           _db.Update("sfPosts", "id", new {Id = 3, Title = "updated"});
                       }, "PetaPoco");
        }
    }
}