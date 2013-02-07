using System;
using System.Collections.Generic;
using Benchmark.Massive.Massive;
using CavemanTools.Testing;
using Tests;
using System.Linq;


namespace Benchmark.Tests
{
    public class MssiveTests : PerformanceTests
    {
        private DynamicModel _db;

        public MssiveTests()
        {
            _db = new DynamicModel(Config.ConnectionName, "sfPosts", "Id");
        }

        public override void FetchSingleEntity(BenchmarksContainer bc)
        {

            bc.Add(i =>
                       {
                           throw new NotSupportedException("not explicit type support");
                       }, "Massive");
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(i =>
                       {
                           _db.Single(5);
                       }, "Massive");
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(i =>
                       {
                           throw new NotSupportedException("not explicit type support");
                       }, "Massive");
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(i =>
                {
                Enumerable.ToArray(_db.Paged(pageSize: 10).Items);
            }, "Massive");
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            QueryTop10Dynamic(bc);
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(i=>
                       {
                           _db.Scalar("select title from sfPosts where id=@0", 5);
                       },"Massive");
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(d=>
                       {
                           throw new NotSupportedException();
                       },"Massive");
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(d =>
                       {
                      throw new NotSupportedException("Couldn't figure how to insert pocos with auto increment id");
                       }, "massive");

        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(d =>
                       {
                           _db.Update(new {title = "Updated"}, 5);
                       }, "massive");
        }

      
    }
}