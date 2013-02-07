using System;
using System.Data;
using CavemanTools.Testing;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using Tests;

namespace Benchmark.Tests
{
    public class ServiceStackTests:PerformanceTests
    {
        private OrmLiteConnectionFactory _db;
        private IDbConnection _cnx;
        private const string Name = "OrmLite";

        public ServiceStackTests()
        {
            _db = new OrmLiteConnectionFactory(Config.Connex, SqlServerOrmLiteDialectProvider.Instance);
            _cnx = _db.OpenDbConnection();
        }
        public override void FetchSingleEntity(BenchmarksContainer bc)
        {
            bc.Add(d=>
                {
                    _cnx.GetById<sfPosts>(5);
                },Name);
        }

        public override void FetchSingleDynamicEntity(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
               _cnx.QuerySingle<dynamic>("select * from sfPOsts where id=@id",new{id=5});
               
            }, Name);
        }

        public override void QueryTop10(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.Query<sfPosts>("select top 10 * from sfPOsts where id>@id", new { id = 5 });                
            }, Name);
        }

        public override void QueryTop10Dynamic(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.Query<dynamic>("select top 10 * from sfPOsts where id>@id", new { id = 5 });                
            }, Name);
        }

        public override void PagedQuery_Skip0_Take10(BenchmarksContainer bc)
        {
            bc.Add(d =>
                {
                    throw new NotSupportedException("No implicit pagination support");
                }, Name);
        }

        public override void ExecuteScalar(BenchmarksContainer bc)
        {
            bc.Add(d =>
            {
                _cnx.GetScalar<string>("select title from sfPOsts where id={0}",5);
                
            }, Name);
        }

        public override void MultiPocoMapping(BenchmarksContainer bc)
        {
            bc.Add(d => { throw new NotSupportedException("Suports only its own specific source format"); }, Name);
        }

        public override void Inserts(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            bc.Add(d=>
                     {
                         _cnx.Insert(p);                          
                     },Name);
        }

        public override void Updates(BenchmarksContainer bc)
        {
            var p = sfPosts.Create();
            p.Id  = 3;
            p.Title = "updated";
            bc.Add(d =>
            {
                _cnx.Update(p);                 
            }, Name);
        }
    }
}