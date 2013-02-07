using System;
using System.Data;
using System.Data.SqlClient;
using CavemanTools.Model;
using CavemanTools.Testing;
using Insight.Database;
using Tests;
using System.Linq;

namespace Benchmark.Tests
{
    public class InsightDbTests : PerformanceTests
    {
        private SqlConnection _cnx;
        private const string Name = "InsightDatabase";
        public InsightDbTests()
        {
            _cnx = Config.GetOpenConnection();
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.QuerySql<sfPosts>("select * from sfPosts where id=@id", new { id = 5 }).FirstOrDefault();
            }, Name);
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.QuerySql("select * from sfPosts where id=@id", new { id = 5 }).FirstOrDefault();
            }, Name);
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.QuerySql<sfPosts>("select top 10 * from sfPosts where id>@id", new { id = 5 }).ToArray();
            }, Name);
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.QuerySql("select top 10 * from sfPosts where id>@id", new { id = 5 }).ToArray();
            }, Name);
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
          bc.Add(d => { throw new NotSupportedException("No implicit pagination support"); }, Name);
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.ExecuteScalarSql<string>("select title from sfPosts where id=@id", new { id = 5 });
            }, Name);
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.QuerySql<PostViewModel, IdName>("select *, Id, title as Name from sfposts p where id=@id order by p.id", new { id = 5 });
            }, Name);
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(id =>
            {
               //var d= _cnx.InsertSql("insert into sfposts (authorid, title, isActive, createdon) values (@authorid, @title, @isactive, @createdon)", p);
                throw new NotSupportedException(); 
            }, Name);
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(id =>
            {
                 throw new NotSupportedException(); 
            }, Name);
        }
    }
}