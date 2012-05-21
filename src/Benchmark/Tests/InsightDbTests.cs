using System;
using System.Data;
using System.Data.SqlClient;
using CavemanTools.Testing;
using Insight.Database;
using Tests;
using System.Linq;

namespace Benchmark.Tests
{
    public class InsightDbTests:PerformanceTests
    {
        private SqlConnection _cnx;
        private const string Name = "InsightDatabase";
        public InsightDbTests()
        {
            _cnx = Config.GetOpenConnection();
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(d=>
                       {
                           _cnx.Query<sfPosts>("select * from sfPosts where id=@id", new {id = 5},commandType:CommandType.Text).FirstOrDefault();
                       },Name);
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.Query("select * from sfPosts where id=@id", new { id = 5 }, commandType: CommandType.Text).FirstOrDefault();
            }, Name);
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.Query<sfPosts>("select top 10 * from sfPosts where id>@id", new { id = 5 }, commandType: CommandType.Text).ToArray();
            }, Name);
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.Query("select top 10 * from sfPosts where id>@id", new { id = 5 }, commandType: CommandType.Text).ToArray();
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
                _cnx.ExecuteScalar<int>("select authorid from sfPosts where id=@id", new { id = 5 }, commandType: CommandType.Text);
            }, Name);
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(d => { throw new NotSupportedException("No implicit multi mapping support"); }, Name);
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            bc.Add(d => { throw new NotSupportedException(); }, Name);
        }

        public override void Updates(BenchmarksContainer bc)
        {
            bc.Add(d => { throw new NotSupportedException(); }, Name);
        }
    }
}