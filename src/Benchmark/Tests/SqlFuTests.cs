using System;
using System.Linq;
using CavemanTools.Testing;
using SqlFu;
using Tests;

namespace Benchmark.Tests
{
    public class SqlFuTests:PerformanceTests,IDisposable
    {
        private SqlFuConnection _db;

        public SqlFuTests()
        {
            _db = new SqlFuConnection(Config.Connex, DbEngine.SqlServer);
            
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(id=>
                       {
                           _db.Get<sfPosts>(5);
                       },"SqlFu Get");
            bc.Add(id=>
                       {
                           _db.QuerySingle<sfPosts>("select * from sfPosts where id=@0",5);
                       },"SqlFu FirstOrDefault");
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.QuerySingle<dynamic>("select * from sfPosts where id=@0", 5);
            }, "SqlFu dynamic");
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(id=>
                       {
                           _db.Query<sfPosts>("select top 10 * from sfposts where id>@0",5);
                       },"SqlFu");
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Query<dynamic>("select top 10 * from sfposts where id>@0", 5);
            }, "SqlFu");
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.PagedQuery<sfPosts>(0,10,"select * from sfposts where id>@0", 5);
            }, "SqlFu");
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.GetValue<string>("select title from sfposts where id=@0", 5);
            }, "SqlFu scalar int");
            
            bc.Add(id =>
            {
                _db.GetColumnValue<sfPosts,string>(p=>p.Title,p=>p.Id==5);
            }, "SqlFu get scalar expression based");

            //bc.Add(id =>
            //{
            //    _db.ExecuteScalar<int?>("select topicid from sfposts where id=@0 order by id", 5);
            //}, "SqlFu scalar null to nullable");
            //bc.Add(id =>
            //{
            //    _db.ExecuteScalar<PostType>("select Type from sfposts where id=@0 order by id", 5);
            //}, "SqlFu scalar enum");
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.QuerySingle<PostViewModel>("select *,id as Author_Id,title as Author_Name from sfposts where id=@0 order by id",5);
            }, "SqlFu");
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(id =>
                       {
                           _db.Insert(p);
                       }, "SqlFu");
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                _db.Update<sfPosts>(new{Id=3,Title="updated"});
            }, "SqlFu");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}